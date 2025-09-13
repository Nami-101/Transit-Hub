-- Transit-Hub Stored Procedures - User Management
-- Created: 2025-09-09

USE TransitHubDB;
GO

-- =============================================
-- User Registration
-- =============================================
CREATE OR ALTER PROCEDURE sp_RegisterUser
    @Name NVARCHAR(100),
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(500),
    @Phone NVARCHAR(15),
    @Age INT,
    @CreatedBy NVARCHAR(100) = 'System'
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if email already exists
        IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email AND IsActive = 1)
        BEGIN
            THROW 50001, 'Email already registered', 1;
        END
        
        -- Check if phone already exists
        IF EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone AND IsActive = 1)
        BEGIN
            THROW 50002, 'Phone number already registered', 1;
        END
        
        -- Validate age
        IF @Age < 0 OR @Age > 120
        BEGIN
            THROW 50003, 'Invalid age. Age must be between 0 and 120', 1;
        END
        
        -- Determine senior citizen status
        DECLARE @IsSeniorCitizen BIT = CASE WHEN @Age >= 60 THEN 1 ELSE 0 END;
        
        -- Insert user
        DECLARE @UserID INT;
        INSERT INTO Users (Name, Email, PasswordHash, Phone, Age, IsSeniorCitizen, IsVerified, CreatedAt, CreatedBy, IsActive)
        VALUES (@Name, @Email, @PasswordHash, @Phone, @Age, @IsSeniorCitizen, 0, GETUTCDATE(), @CreatedBy, 1);
        
        SET @UserID = SCOPE_IDENTITY();
        
        -- Generate verification token
        DECLARE @Token NVARCHAR(500) = NEWID();
        DECLARE @ExpiryDate DATETIME2 = DATEADD(HOUR, 24, GETUTCDATE());
        
        INSERT INTO GmailVerificationTokens (UserID, Token, ExpiryDate, IsUsed, CreatedAt)
        VALUES (@UserID, @Token, @ExpiryDate, 0, GETUTCDATE());
        
        COMMIT TRANSACTION;
        
        -- Return success with user details
        SELECT 
            @UserID as UserID,
            @Token as VerificationToken,
            'User registered successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, CreatedAt)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), GETUTCDATE());
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- User Login
-- =============================================
CREATE OR ALTER PROCEDURE sp_LoginUser
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check if user exists and is active
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = @Email AND IsActive = 1)
        BEGIN
            THROW 50004, 'Invalid email or password', 1;
        END
        
        -- Validate password
        DECLARE @StoredPasswordHash NVARCHAR(500);
        DECLARE @UserID INT;
        DECLARE @IsVerified BIT;
        
        SELECT @StoredPasswordHash = PasswordHash, @UserID = UserID, @IsVerified = IsVerified
        FROM Users 
        WHERE Email = @Email AND IsActive = 1;
        
        IF @StoredPasswordHash != @PasswordHash
        BEGIN
            THROW 50004, 'Invalid email or password', 1;
        END
        
        IF @IsVerified = 0
        BEGIN
            THROW 50005, 'Email not verified. Please check your email for verification link', 1;
        END
        
        -- Return user details
        SELECT 
            UserID,
            Name,
            Email,
            Phone,
            Age,
            IsSeniorCitizen,
            'Login successful' as Message,
            1 as Success
        FROM Users 
        WHERE UserID = @UserID;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID, CreatedAt)
        VALUES ('Warning', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID, GETUTCDATE());
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Verify Email
-- =============================================
CREATE OR ALTER PROCEDURE sp_VerifyEmail
    @Token NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if token exists and is valid
        DECLARE @UserID INT;
        DECLARE @ExpiryDate DATETIME2;
        DECLARE @IsUsed BIT;
        
        SELECT @UserID = UserID, @ExpiryDate = ExpiryDate, @IsUsed = IsUsed
        FROM GmailVerificationTokens 
        WHERE Token = @Token;
        
        IF @UserID IS NULL
        BEGIN
            THROW 50006, 'Invalid verification token', 1;
        END
        
        IF @IsUsed = 1
        BEGIN
            THROW 50007, 'Verification token already used', 1;
        END
        
        IF @ExpiryDate < GETUTCDATE()
        BEGIN
            THROW 50008, 'Verification token expired', 1;
        END
        
        -- Mark user as verified
        UPDATE Users 
        SET IsVerified = 1, UpdatedAt = GETUTCDATE(), UpdatedBy = 'EmailVerification'
        WHERE UserID = @UserID;
        
        -- Mark token as used
        UPDATE GmailVerificationTokens 
        SET IsUsed = 1
        WHERE Token = @Token;
        
        COMMIT TRANSACTION;
        
        SELECT 
            'Email verified successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID, CreatedAt)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID, GETUTCDATE());
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Get User Profile
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetUserProfile
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check if user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID AND IsActive = 1)
        BEGIN
            THROW 50009, 'User not found', 1;
        END
        
        -- Return user profile with booking statistics
        SELECT 
            u.UserID,
            u.Name,
            u.Email,
            u.Phone,
            u.Age,
            u.IsSeniorCitizen,
            u.IsVerified,
            u.CreatedAt,
            COUNT(b.BookingID) as TotalBookings,
            COUNT(CASE WHEN bs.StatusName = 'Confirmed' THEN 1 END) as ConfirmedBookings,
            COUNT(CASE WHEN bs.StatusName = 'Cancelled' THEN 1 END) as CancelledBookings,
            COUNT(CASE WHEN bs.StatusName = 'Waitlisted' THEN 1 END) as WaitlistedBookings
        FROM Users u
        LEFT JOIN Bookings b ON u.UserID = b.UserID AND b.IsActive = 1
        LEFT JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID
        WHERE u.UserID = @UserID AND u.IsActive = 1
        GROUP BY u.UserID, u.Name, u.Email, u.Phone, u.Age, u.IsSeniorCitizen, u.IsVerified, u.CreatedAt;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID, CreatedAt)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID, GETUTCDATE());
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Update User Profile
-- =============================================
CREATE OR ALTER PROCEDURE sp_UpdateUserProfile
    @UserID INT,
    @Name NVARCHAR(100),
    @Phone NVARCHAR(15),
    @Age INT,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Check if user exists
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID AND IsActive = 1)
        BEGIN
            THROW 50009, 'User not found', 1;
        END
        
        -- Check if phone is already used by another user
        IF EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone AND UserID != @UserID AND IsActive = 1)
        BEGIN
            THROW 50002, 'Phone number already registered', 1;
        END
        
        -- Validate age
        IF @Age < 0 OR @Age > 120
        BEGIN
            THROW 50003, 'Invalid age. Age must be between 0 and 120', 1;
        END
        
        -- Determine senior citizen status
        DECLARE @IsSeniorCitizen BIT = CASE WHEN @Age >= 60 THEN 1 ELSE 0 END;
        
        -- Update user
        UPDATE Users 
        SET 
            Name = @Name,
            Phone = @Phone,
            Age = @Age,
            IsSeniorCitizen = @IsSeniorCitizen,
            UpdatedBy = @UpdatedBy,
            UpdatedAt = GETUTCDATE()
        WHERE UserID = @UserID;
        
        COMMIT TRANSACTION;
        
        SELECT 
            'Profile updated successfully' as Message,
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