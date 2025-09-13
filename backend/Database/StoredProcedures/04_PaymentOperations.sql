-- Transit-Hub Stored Procedures - Payment Operations
-- Created: 2025-09-09

USE TransitHubDB;
GO

-- =============================================
-- Process Payment
-- =============================================
CREATE OR ALTER PROCEDURE sp_ProcessPayment
    @BookingID INT,
    @PaymentModeID INT,
    @Amount DECIMAL(10,2),
    @TransactionRef NVARCHAR(100) = NULL,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate booking
        DECLARE @BookingAmount DECIMAL(10,2), @BookingStatus NVARCHAR(50), @UserID INT;
        
        SELECT @BookingAmount = b.TotalAmount, @BookingStatus = bs.StatusName, @UserID = b.UserID
        FROM Bookings b
        INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID
        WHERE b.BookingID = @BookingID AND b.IsActive = 1;
        
        IF @BookingAmount IS NULL
        BEGIN
            THROW 50023, 'Invalid booking ID', 1;
        END
        
        -- Check if booking is in valid state for payment
        IF @BookingStatus NOT IN ('Confirmed', 'Waitlisted')
        BEGIN
            THROW 50024, 'Payment not allowed for cancelled bookings', 1;
        END
        
        -- Validate payment amount
        IF @Amount != @BookingAmount
        BEGIN
            THROW 50025, 'Payment amount does not match booking amount', 1;
        END
        
        -- Check if payment already exists
        IF EXISTS (SELECT 1 FROM Payments WHERE BookingID = @BookingID AND Status = 'Success')
        BEGIN
            THROW 50026, 'Payment already completed for this booking', 1;
        END
        
        -- Validate payment mode
        IF NOT EXISTS (SELECT 1 FROM PaymentModes WHERE PaymentModeID = @PaymentModeID AND IsActive = 1)
        BEGIN
            THROW 50027, 'Invalid payment mode', 1;
        END
        
        -- Generate transaction reference if not provided
        IF @TransactionRef IS NULL
        BEGIN
            SET @TransactionRef = 'TXN' + FORMAT(GETDATE(), 'yyyyMMddHHmmss') + RIGHT('0000' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR(4)), 4);
        END
        
        -- Simulate payment processing (in real scenario, this would call payment gateway)
        DECLARE @PaymentStatus NVARCHAR(20);
        DECLARE @RandomSuccess INT = ABS(CHECKSUM(NEWID())) % 100;
        
        -- 95% success rate for simulation
        IF @RandomSuccess < 95
        BEGIN
            SET @PaymentStatus = 'Success';
        END
        ELSE
        BEGIN
            SET @PaymentStatus = 'Failed';
        END
        
        -- Insert payment record
        DECLARE @PaymentID INT;
        INSERT INTO Payments (BookingID, PaymentModeID, Amount, Status, TransactionRef, CreatedBy)
        VALUES (@BookingID, @PaymentModeID, @Amount, @PaymentStatus, @TransactionRef, @CreatedBy);
        
        SET @PaymentID = SCOPE_IDENTITY();
        
        -- Create audit log
        INSERT INTO BookingAudit (BookingID, Action, ActionBy, Details, CreatedBy)
        VALUES (@BookingID, 'Payment ' + @PaymentStatus, @CreatedBy, 
                'Payment of ₹' + CAST(@Amount AS NVARCHAR(20)) + ' via ' + 
                (SELECT ModeName FROM PaymentModes WHERE PaymentModeID = @PaymentModeID), @CreatedBy);
        
        -- Create notification
        DECLARE @NotificationTitle NVARCHAR(200);
        DECLARE @NotificationMessage NVARCHAR(1000);
        DECLARE @NotificationType NVARCHAR(50);
        
        IF @PaymentStatus = 'Success'
        BEGIN
            SET @NotificationTitle = 'Payment Successful';
            SET @NotificationMessage = 'Your payment of ₹' + CAST(@Amount AS NVARCHAR(20)) + ' has been processed successfully. Transaction ID: ' + @TransactionRef;
            SET @NotificationType = 'Success';
        END
        ELSE
        BEGIN
            SET @NotificationTitle = 'Payment Failed';
            SET @NotificationMessage = 'Your payment of ₹' + CAST(@Amount AS NVARCHAR(20)) + ' could not be processed. Please try again.';
            SET @NotificationType = 'Error';
        END
        
        INSERT INTO Notifications (UserID, Title, Message, Type, RelatedBookingID)
        VALUES (@UserID, @NotificationTitle, @NotificationMessage, @NotificationType, @BookingID);
        
        COMMIT TRANSACTION;
        
        -- Return payment result
        SELECT 
            @PaymentID as PaymentID,
            @PaymentStatus as Status,
            @TransactionRef as TransactionReference,
            @Amount as Amount,
            CASE WHEN @PaymentStatus = 'Success' THEN 'Payment processed successfully' 
                 ELSE 'Payment failed. Please try again' END as Message,
            CASE WHEN @PaymentStatus = 'Success' THEN 1 ELSE 0 END as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Get Payment History
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetPaymentHistory
    @UserID INT = NULL,
    @BookingID INT = NULL,
    @Status NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate inputs
        IF @UserID IS NULL AND @BookingID IS NULL
        BEGIN
            THROW 50028, 'Either UserID or BookingID must be provided', 1;
        END
        
        SELECT 
            p.PaymentID,
            p.BookingID,
            b.BookingReference,
            b.BookingType,
            p.Amount,
            p.Status,
            p.TransactionRef,
            p.PaymentDate,
            pm.ModeName as PaymentMode,
            u.Name as UserName,
            u.Email as UserEmail
        FROM Payments p
        INNER JOIN Bookings b ON p.BookingID = b.BookingID
        INNER JOIN Users u ON b.UserID = u.UserID
        INNER JOIN PaymentModes pm ON p.PaymentModeID = pm.PaymentModeID
        WHERE 
            (@UserID IS NULL OR b.UserID = @UserID)
            AND (@BookingID IS NULL OR p.BookingID = @BookingID)
            AND (@Status IS NULL OR p.Status = @Status)
            AND p.IsActive = 1
        ORDER BY p.PaymentDate DESC;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Process Refund
-- =============================================
CREATE OR ALTER PROCEDURE sp_ProcessRefund
    @BookingID INT,
    @RefundAmount DECIMAL(10,2),
    @CancellationFee DECIMAL(10,2) = 0,
    @CancellationReason NVARCHAR(500) = NULL,
    @ProcessedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate booking and get payment details
        DECLARE @UserID INT, @BookingAmount DECIMAL(10,2), @BookingStatus NVARCHAR(50);
        DECLARE @PaymentID INT, @PaymentAmount DECIMAL(10,2), @PaymentStatus NVARCHAR(20);
        
        SELECT @UserID = b.UserID, @BookingAmount = b.TotalAmount, @BookingStatus = bs.StatusName
        FROM Bookings b
        INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID
        WHERE b.BookingID = @BookingID AND b.IsActive = 1;
        
        IF @UserID IS NULL
        BEGIN
            THROW 50023, 'Invalid booking ID', 1;
        END
        
        -- Get successful payment for this booking
        SELECT @PaymentID = PaymentID, @PaymentAmount = Amount, @PaymentStatus = Status
        FROM Payments 
        WHERE BookingID = @BookingID AND Status = 'Success' AND IsActive = 1;
        
        IF @PaymentID IS NULL
        BEGIN
            THROW 50029, 'No successful payment found for this booking', 1;
        END
        
        -- Validate refund amount
        IF @RefundAmount + @CancellationFee != @PaymentAmount
        BEGIN
            THROW 50030, 'Refund amount plus cancellation fee must equal payment amount', 1;
        END
        
        -- Check if refund already processed
        IF EXISTS (SELECT 1 FROM Payments WHERE BookingID = @BookingID AND Status = 'Refunded')
        BEGIN
            THROW 50031, 'Refund already processed for this booking', 1;
        END
        
        -- Create refund payment record
        DECLARE @RefundPaymentID INT;
        INSERT INTO Payments (BookingID, PaymentModeID, Amount, Status, TransactionRef, CreatedBy)
        SELECT @BookingID, PaymentModeID, -@RefundAmount, 'Refunded', 
               'REF' + FORMAT(GETDATE(), 'yyyyMMddHHmmss') + RIGHT('0000' + CAST(ABS(CHECKSUM(NEWID())) % 10000 AS NVARCHAR(4)), 4),
               @ProcessedBy
        FROM Payments WHERE PaymentID = @PaymentID;
        
        SET @RefundPaymentID = SCOPE_IDENTITY();
        
        -- Create cancellation record
        INSERT INTO Cancellations (BookingID, CancellationReason, RefundAmount, CancellationFee, CancelledBy, CreatedBy)
        VALUES (@BookingID, @CancellationReason, @RefundAmount, @CancellationFee, @ProcessedBy, @ProcessedBy);
        
        -- Update booking status to cancelled
        DECLARE @CancelledStatusID INT;
        SELECT @CancelledStatusID = StatusID FROM BookingStatusTypes WHERE StatusName = 'Cancelled';
        
        UPDATE Bookings 
        SET StatusID = @CancelledStatusID, UpdatedBy = @ProcessedBy, UpdatedAt = GETUTCDATE()
        WHERE BookingID = @BookingID;
        
        -- Create audit log
        INSERT INTO BookingAudit (BookingID, Action, ActionBy, Details, CreatedBy)
        VALUES (@BookingID, 'Refund Processed', @ProcessedBy, 
                'Refund of ₹' + CAST(@RefundAmount AS NVARCHAR(20)) + ' processed. Cancellation fee: ₹' + CAST(@CancellationFee AS NVARCHAR(20)), @ProcessedBy);
        
        -- Create notification
        INSERT INTO Notifications (UserID, Title, Message, Type, RelatedBookingID)
        VALUES (@UserID, 'Refund Processed', 
                'Your refund of ₹' + CAST(@RefundAmount AS NVARCHAR(20)) + ' has been processed and will be credited to your account within 5-7 business days.',
                'Info', @BookingID);
        
        COMMIT TRANSACTION;
        
        -- Return refund details
        SELECT 
            @RefundPaymentID as RefundPaymentID,
            @RefundAmount as RefundAmount,
            @CancellationFee as CancellationFee,
            'Refund processed successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO