CREATE TABLE users
(
	discord_id TEXT PRIMARY KEY,
	discord_username TEXT NOT NULL,
	avatar_url TEXT NOT NULL DEFAULT '',
	role TEXT NOT NULL
		CHECK (role IN ('Player', 'Mod', 'Admin'))
		DEFAULT 'Player',
		
	game_token UUID NOT NULL UNIQUE
		DEFAULT gen_random_uuid(),
	
	-- Whether this user was converted from the legacy auth system.
	consumed_legacy_token BOOLEAN NOT NULL DEFAULT FALSE,
		
	badges INTEGER[] NOT NULL DEFAULT '{}',
	
	joined_at TIMESTAMP NOT NULL DEFAULT NOW(),
	last_login_at TIMESTAMP NOT NULL DEFAULT NOW()
)