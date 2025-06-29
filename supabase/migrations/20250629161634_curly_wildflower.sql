-- AuthFlowPro Database Initialization Script
-- This script sets up the initial database structure

-- Create database if it doesn't exist
-- Note: This is handled by the POSTGRES_DB environment variable in Docker

-- Set timezone
SET timezone = 'UTC';

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- The actual tables will be created by Entity Framework migrations
-- This script is just for any additional setup if needed

-- Log the initialization
DO $$
BEGIN
    RAISE NOTICE 'AuthFlowPro database initialized successfully';
END $$;