CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    username VARCHAR(50) NOT NULL,
    display_name VARCHAR(100) NOT NULL,
    phone_number VARCHAR(15) UNIQUE NOT NULL,
    status VARCHAR(20) DEFAULT 'ACTIVE' NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CHECK (length(username) >= 3 AND length(username) <= 50),
    CHECK (username ~ '^[a-zA-Z0-9_]+$'),
    CHECK (length(phone_number) >= 10 AND length(phone_number) <= 15),
    CHECK (status IN ('ACTIVE', 'INACTIVE', 'BANNED', 'DELETED'))
);

CREATE TABLE user_profiles (
    user_id UUID PRIMARY KEY REFERENCES users(user_id) ON DELETE CASCADE,
    bio VARCHAR(500),
    avatar_url VARCHAR(255),
    privacy_setting VARCHAR(20) DEFAULT 'EVERYONE' NOT NULL,
    
    CHECK (privacy_setting IN ('EVERYONE', 'CONTACTS', 'NOBODY'))
);

CREATE TABLE groups (
    group_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    created_by UUID NOT NULL REFERENCES users(user_id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CHECK (length(name) >= 1 AND length(name) <= 100)
);

CREATE TABLE group_members (
    group_id UUID NOT NULL REFERENCES groups(group_id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    role VARCHAR(20) DEFAULT 'MEMBER' NOT NULL,
    joined_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (group_id, user_id),
    CHECK (role IN ('ADMIN', 'MODERATOR', 'MEMBER'))
);

CREATE TABLE messages (
    message_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    sender_id UUID NOT NULL REFERENCES users(user_id),
    receiver_id UUID REFERENCES users(user_id), -- Null if group message
    group_id UUID REFERENCES groups(group_id), -- Null if direct message
    content TEXT NOT NULL,
    message_type VARCHAR(20) DEFAULT 'TEXT' NOT NULL,
    sent_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    read_at TIMESTAMP WITH TIME ZONE,
    
    CHECK (length(content) > 0 AND length(content) <= 4000),
    CHECK (message_type IN ('TEXT', 'IMAGE', 'VIDEO', 'AUDIO', 'FILE')),
    -- Must be either a direct message or a group message, not both, not neither
    CHECK ((receiver_id IS NOT NULL AND group_id IS NULL) OR (receiver_id IS NULL AND group_id IS NOT NULL)),
    CHECK (read_at IS NULL OR read_at >= sent_at)
);
