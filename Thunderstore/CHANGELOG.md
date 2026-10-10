## 1.8.0
- Invisible players are now visible to moderators.
- Custom packets can now be sent globally across stages. Contributed by @AbsentmindedGCN

## 1.7.1
- Fixed "emojis" command reporting the wrong total number of pages.

## 1.7.0
- Emojis are no longer case sensitive.
- The "emojis" chat command now displays pages, rather than the full list of emojis.
- There is a new "emoji_search (query)" chat command, to search emojis by a query.
- Fixed confirmation prompt overlapping player list when banning a player.
- Holding left shift when banning a player from the player list skips the confirmation prompt.
- Sent chat history: Pressing up/down with the chat box open now scrolls through your previously sent messages.
- Updated badges and emojis.

## 1.6.1
- Ragdolls now work for way more characters without a BRC skeleton. Some might still not work if they are missing certain humanoid bones or built weirdly.
- Player IDs are now displayed on the player list and spectator mode.
- Fixed input field for crew name being mislabeled as player name.
- Fixed input field inconsistent focus bug.
- Removed rich text tags from input fields.
- ServerApp: Can now disable custom packets.
- Updated badges and emojis.

## 1.6.0
- Added a Settings button to the Multiplayer phone app that allows you to change your name and other common config settings directly in-game.
- When banning players from the player list, you are now asked for confirmation.
- Current theme now reloads on stage transition.
- 2 New themes are now bundled with the mod by default.
Themes can be changed via the aforementioned new Settings option on the phone.
- Fixed spectator exceptions and FirstPersonFunk compatibility issues.
- Players no longer slide after unragdolling.
- Updated emojis.
- There is a new PvP setting. Players that have PvP on will show up as yellow on the map and can fight eachother.
- Big Slopper onboarding: New players will be told how to change their name and set up their mod via chat automatically.

## 1.5.1
- When PvP is enabled on the server, it now only takes effect on freeroam. If you're in a lobby you will always be unaffected by PvP, regardless of whether you're currently playing a gamemode or not.
- Reworked the way traffic car hitboxes work to make it easier and more consistent to get hit by cars. This only takes effect when outside of a lobby in freeroam, and can be toggled on and off via the config.
- Default ragdoll key is now X instead of K.
- Improved the quality of ragdoll physics and collision detection a bit.
- Fixed a memory leak caused by visual effects that would degrade performance over time.
- Fixed many crashes caused by being in spectator mode.
- Fixed teleports not working when using Woodz Warper and being in the ragdoll state.
- Fixed the ragdoll key sometimes not working at higher framerates.
- Fixed ragdolls starting in a reference pose when ragdolling with your movestyle equipped.
- Fixed odd behavior when being damaged while in the ragdoll state, and improved ragdoll hitboxes.
- Fixed being able to ragdoll in input disabled situations.
- Updated some badges.
- Updated profanity filter to lessen false positives.