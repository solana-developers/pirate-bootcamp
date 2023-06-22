pub use crate::errors::SevenSeasError;
use crate::PLAYER_KILL_REWARD;
use crate::{Ship, CHEST_REWARD};
use anchor_lang::prelude::*;
use anchor_spl::token::Transfer;

const BOARD_SIZE_X: usize = 10;
const BOARD_SIZE_Y: usize = 10;

const STATE_EMPTY: u8 = 0;
const STATE_PLAYER: u8 = 1;
const STATE_CHEST: u8 = 2;

const GAME_ACTION_SHIP_SHOT: u8 = 0;
const GAME_ACTION_SHIP_TAKEN_DAMAGE: u8 = 1;
const GAME_ACTION_SHIP_CTHULUH_ATTACKED_SHIP: u8 = 2;
const GAME_ACTION_SHIP_COINS_COLLECTED: u8 = 3;

const CHEST_COIN_REWARD: u64 = 10;
const DESTROY_SHIP_COIN_REWARD: u64 = 10;

pub const TOKEN_DECIMAL_MULTIPLIER: u64 = 1000000000;

#[derive(Accounts)]
pub struct Reset<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,
    // We must specify the space in order to initialize an account.
    // First 8 bytes are default account discriminator,
    #[account(
        mut,
        seeds = [b"level"],
        bump,
    )]
    pub game_data_account: AccountLoader<'info, GameDataAccount>,
}

#[derive(Accounts)]
pub struct ResetShip<'info> {
    #[account(mut)]
    pub signer: Signer<'info>,
    // We must specify the space in order to initialize an account.
    // First 8 bytes are default account discriminator,
    #[account(
        mut,
        seeds = [b"level"],
        bump,
    )]
    pub game_data_account: AccountLoader<'info, GameDataAccount>,
}

#[account(zero_copy(unsafe))]
#[repr(packed)]
#[derive(Default)]
pub struct GameDataAccount {
    board: [[Tile; BOARD_SIZE_X]; BOARD_SIZE_Y],
    action_id: u64,
}

#[zero_copy(unsafe)]
#[repr(packed)]
#[derive(Default)]
pub struct Tile {
    player: Pubkey,      // 32
    state: u8,           // 1
    health: u64,         // 8
    damage: u64,         // 8
    range: u16,          // 2
    collect_reward: u64, // 8
    avatar: Pubkey,      // 32 used in the client to display the avatar
    look_direction: u8,  // 1 (Up, right, down, left)
    ship_level: u16,     // 2
    start_health: u64,   // 8 (used to calculate the length of the health bar in the client)
}

#[account]
pub struct GameActionHistory {
    id_counter: u64,
    game_actions: Vec<GameAction>,
}

#[derive(Debug, Clone, AnchorSerialize, AnchorDeserialize)]
pub struct GameAction {
    action_id: u64,  // 1
    action_type: u8, // 1
    player: Pubkey,  // 32
    target: Pubkey,  // 32
    damage: u64,     // 8
}

impl GameDataAccount {
    pub fn print(&mut self) -> Result<()> {
        // print will only work locally for debugging otherwise it eats too much compute
        /*
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y  ];
                if tile.state == STATE_EMPTY {
                    msg!("empty")
                } else {
                    msg!("{} {}", tile.player, tile.state)
                }
            }
        }*/

        Ok(())
    }

    pub fn reset(&mut self) -> Result<()> {
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                self.board[x][y].state = STATE_EMPTY
            }
        }
        Ok(())
    }

    pub fn reset_ship(&mut self, ship_owner: Pubkey) -> Result<()> {
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                if self.board[x][y].player == ship_owner && self.board[x][y].state == STATE_PLAYER {
                    self.board[x][y].state = STATE_EMPTY
                }
            }
        }
        Ok(())
    }

    pub fn euclidean_distance(x1: &usize, x2: &usize, y1: &usize, y2: &usize) -> f64 {
        let dx = (x1 - x2) as f64;
        let dy = (y1 - y2) as f64;
        (dx * dx + dy * dy).sqrt()
    }

    pub fn cthulhu<'info>(
        &mut self,
        _player: AccountInfo,
        game_actions: &mut GameActionHistory,
        _chest_vault: AccountInfo,
        _vault_token_account: AccountInfo<'info>,
        _player_token_account: AccountInfo<'info>,
        _token_account_owner_pda: AccountInfo<'info>,
        _token_program: AccountInfo<'info>,
        _token_owner_bump: u8,
    ) -> Result<()> {
        let mut smallest_distance: f64 = 100000.0;
        let mut attacked_player_position: Option<(usize, usize)> = None;

        let cthulhu_position: (usize, usize) = (0, 0);

        // Find closest player on the board
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y];
                if tile.state == STATE_PLAYER {
                    let distance =
                        Self::euclidean_distance(&x, &cthulhu_position.0, &y, &cthulhu_position.0);
                    if distance < smallest_distance {
                        smallest_distance = distance;
                        attacked_player_position = Some((x, y));
                    }
                }
            }
        }

        // Found a player on the board that we can attack
        match attacked_player_position {
            None => {
                return Err(SevenSeasError::CouldNotFindAShipToAttack.into());
            }
            Some(val) => {
                let mut tile = &mut self.board[val.0][val.1];

                let mut rng = XorShift64 {
                    a: tile.health as u64,
                };

                let chtulu_damage: u64 = 10;
                let damage_variant = ((chtulu_damage as f64) * 0.3).ceil() as u64;
                let damage = chtulu_damage + ((rng.next() % damage_variant) + 1);
                let option = tile.health.checked_sub(damage);
                match option {
                    None => {
                        tile.health = 0;
                    }
                    Some(val) => {
                        tile.health = val;
                    }
                }

                if tile.health == 0 {
                    tile.state = STATE_EMPTY;
                }

                let item = GameAction {
                    action_id: self.action_id,
                    action_type: GAME_ACTION_SHIP_CTHULUH_ATTACKED_SHIP,
                    player: tile.player.key(),
                    target: tile.player.key(),
                    damage: damage,
                };
                self.add_new_game_action(game_actions, item);

                msg!(
                    "Attack closes enemy is at {} {} with damage {}",
                    val.0,
                    val.1,
                    damage
                );
            }
        }

        Ok(())
    }

    pub fn shoot<'info>(
        &mut self,
        player: AccountInfo,
        game_actions: &mut GameActionHistory,
        chest_vault: AccountInfo,
        vault_token_account: AccountInfo<'info>,
        player_token_account: AccountInfo<'info>,
        token_account_owner_pda: AccountInfo<'info>,
        token_program: AccountInfo<'info>,
        token_owner_bump: u8,
    ) -> Result<()> {
        let mut player_position: Option<(usize, usize)> = None;

        // Find the player on the board
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y];
                if tile.state == STATE_PLAYER {
                    if tile.player == player.key.clone() {
                        player_position = Some((x, y));
                    }
                    msg!("{} {}", tile.player, tile.state);
                }
            }
        }

        // If the player is on the board shoot
        match player_position {
            None => {
                return Err(SevenSeasError::TriedToShootWithPlayerThatWasNotOnTheBoard.into());
            }
            Some(val) => {
                msg!("Player position x:{} y:{}", val.0, val.1);
                let player_tile: Tile = self.board[val.0][val.1];
                let range_usize: usize = usize::from(player_tile.range);
                let damage = player_tile.damage + 2;
                for range in 1..range_usize + 1 {
                    // Shoot left
                    if player_tile.look_direction % 2 == 0 && val.0 >= range {
                        self.attack_tile(
                            (val.0 - range, val.1),
                            damage,
                            player.clone(),
                            chest_vault.clone(),
                            game_actions,
                            &vault_token_account,
                            &player_token_account,
                            &token_account_owner_pda,
                            &token_program,
                            token_owner_bump,
                        )?;
                    }

                    // Shoot right
                    if player_tile.look_direction % 2 == 0 && val.0 < BOARD_SIZE_X - range {
                        self.attack_tile(
                            (val.0 + range, val.1),
                            damage,
                            player.clone(),
                            chest_vault.clone(),
                            game_actions,
                            &vault_token_account,
                            &player_token_account,
                            &token_account_owner_pda,
                            &token_program,
                            token_owner_bump,
                        )?;
                    }

                    // Shoot down
                    if player_tile.look_direction % 2 == 1 && val.1 < BOARD_SIZE_Y - range {
                        self.attack_tile(
                            (val.0, val.1 + range),
                            damage,
                            player.clone(),
                            chest_vault.clone(),
                            game_actions,
                            &vault_token_account,
                            &player_token_account,
                            &token_account_owner_pda,
                            &token_program,
                            token_owner_bump,
                        )?;
                    }

                    // Shoot up
                    if player_tile.look_direction % 2 == 1 && val.1 >= range {
                        self.attack_tile(
                            (val.0, val.1 - range),
                            damage,
                            player.clone(),
                            chest_vault.clone(),
                            game_actions,
                            &vault_token_account,
                            &player_token_account,
                            &token_account_owner_pda,
                            &token_program,
                            token_owner_bump,
                        )?;
                    }
                }

                let item = GameAction {
                    action_id: self.action_id,
                    action_type: GAME_ACTION_SHIP_SHOT,
                    player: player.key(),
                    target: player.key(),
                    damage: damage,
                };
                self.add_new_game_action(game_actions, item);
            }
        }

        Ok(())
    }

    fn add_new_game_action(
        &mut self,
        game_actions: &mut GameActionHistory,
        game_action: GameAction,
    ) {
        {
            let option_add = self.action_id.checked_add(1);
            match option_add {
                Some(val) => {
                    self.action_id = val;
                }
                None => {
                    self.action_id = 0;
                }
            }
        }
        game_actions.game_actions.push(game_action);
        if game_actions.game_actions.len() > 30 {
            game_actions.game_actions.drain(0..5);
        }
    }

    fn attack_tile<'info>(
        &mut self,
        attacked_position: (usize, usize),
        damage: u64,
        attacker: AccountInfo,
        chest_vault: AccountInfo,
        game_actions: &mut GameActionHistory,
        vault_token_account: &AccountInfo<'info>,
        player_token_account: &AccountInfo<'info>,
        token_account_owner_pda: &AccountInfo<'info>,
        token_program: &AccountInfo<'info>,
        token_owner_bump: u8,
    ) -> Result<()> {
        let mut attacked_tile: Tile = self.board[attacked_position.0][attacked_position.1];
        msg!("Attack x:{} y:{}", attacked_position.0, attacked_position.1);

        let transfer_instruction = Transfer {
            from: vault_token_account.to_account_info(),
            to: player_token_account.to_account_info(),
            authority: token_account_owner_pda.to_account_info(),
        };

        let seeds = &[b"token_account_owner_pda".as_ref(), &[token_owner_bump]];
        let signer = &[&seeds[..]];

        let cpi_ctx = CpiContext::new_with_signer(
            token_program.to_account_info(),
            transfer_instruction,
            signer,
        );

        if attacked_tile.state == STATE_PLAYER {
            let match_option = attacked_tile.health.checked_sub(damage);
            match match_option {
                None => {
                    attacked_tile.health = 0;
                    self.on_ship_died(attacked_position, attacked_tile, chest_vault, &attacker)?;
                    anchor_spl::token::transfer(
                        cpi_ctx,
                        (attacked_tile.ship_level as u64)
                            * DESTROY_SHIP_COIN_REWARD
                            * TOKEN_DECIMAL_MULTIPLIER,
                    )?;

                    let new_game_action = GameAction {
                        action_id: self.action_id,
                        action_type: GAME_ACTION_SHIP_COINS_COLLECTED,
                        player: attacker.key(),
                        target: attacked_tile.player.key(),
                        damage: DESTROY_SHIP_COIN_REWARD,
                    };
                    self.add_new_game_action(game_actions, new_game_action);
                }
                Some(value) => {
                    msg!("New health {}", value);
                    self.board[attacked_position.0][attacked_position.1].health = value;
                    if value == 0 {
                        self.on_ship_died(
                            attacked_position,
                            attacked_tile,
                            chest_vault,
                            &attacker,
                        )?;
                        anchor_spl::token::transfer(
                            cpi_ctx,
                            (attacked_tile.ship_level as u64)
                                * DESTROY_SHIP_COIN_REWARD
                                * TOKEN_DECIMAL_MULTIPLIER,
                        )?;
                        let item = GameAction {
                            action_id: self.action_id,
                            action_type: GAME_ACTION_SHIP_COINS_COLLECTED,
                            player: attacker.key(),
                            target: attacked_tile.player.key(),
                            damage: DESTROY_SHIP_COIN_REWARD,
                        };
                        self.add_new_game_action(game_actions, item);
                    }
                }
            };
            let item = GameAction {
                action_id: self.action_id,
                action_type: GAME_ACTION_SHIP_TAKEN_DAMAGE,
                player: attacker.key(),
                target: attacked_tile.player.key(),
                damage: damage,
            };
            self.add_new_game_action(game_actions, item);
        }
        Ok(())
    }

    fn on_ship_died(
        &mut self,
        attacked_position: (usize, usize),
        attacked_tile: Tile,
        chest_vault: AccountInfo,
        attacker: &AccountInfo,
    ) -> Result<()> {
        msg!(
            "Enemy killed x:{} y:{} pubkey: {}",
            attacked_position.0,
            attacked_position.1,
            attacked_tile.player
        );
        self.board[attacked_position.0][attacked_position.1].state = STATE_EMPTY;
        **chest_vault.try_borrow_mut_lamports()? -= attacked_tile.collect_reward;
        **attacker.try_borrow_mut_lamports()? += attacked_tile.collect_reward;
        Ok(())
    }

    pub fn move_in_direction_by_thread<'info>(&mut self) -> Result<()> {
        let mut alive_players: Vec<(usize, usize)> = Vec::new();

        // Find the player on the board
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                if self.board[x][y].state == STATE_PLAYER {
                    let new_position: (usize, usize) = (x, y);
                    alive_players.push(new_position)
                }
            }
        }

        for player in alive_players {
            if self.board[player.0][player.1].state == STATE_PLAYER {
                let mut new_position: (usize, usize) = (player.0, player.1);
                msg!("Player found at x:{} y:{}", player.0, player.1);
                match self.board[player.0][player.1].look_direction {
                    // Up
                    0 => {
                        if new_position.1 == 0 {
                            new_position.1 = BOARD_SIZE_Y - 1;
                        } else {
                            new_position.1 -= 1;
                        }
                    }
                    // Right
                    1 => {
                        if new_position.0 == BOARD_SIZE_X - 1 {
                            new_position.0 = 0;
                        } else {
                            new_position.0 += 1;
                        }
                    }
                    // Down
                    2 => {
                        if new_position.1 == BOARD_SIZE_Y - 1 {
                            new_position.1 = 0;
                        } else {
                            new_position.1 += 1;
                        }
                    }
                    // Left
                    3 => {
                        if new_position.0 == 0 {
                            new_position.0 = BOARD_SIZE_X - 1;
                        } else {
                            new_position.0 -= 1;
                        }
                    }
                    _ => {
                        return Err(SevenSeasError::WrongDirectionInput.into());
                    }
                }

                if self.board[new_position.0][new_position.1].state == STATE_EMPTY {
                    msg!("Move to x:{} y:{}", new_position.0, new_position.1);
                    self.board[new_position.0][new_position.1] = self.board[player.0][player.1];
                    self.board[player.0][player.1].state = STATE_EMPTY;
                }
            }
        }

        Ok(())
    }

    pub fn move_in_direction<'info>(
        &mut self,
        direction: u8,
        player: AccountInfo,
        chest_vault: AccountInfo,
        vault_token_account: AccountInfo<'info>,
        player_token_account: AccountInfo<'info>,
        token_account_owner_pda: AccountInfo<'info>,
        token_program: AccountInfo<'info>,
        token_owner_bump: u8,
        game_actions: &mut GameActionHistory,
    ) -> Result<()> {
        let mut player_position: Option<(usize, usize)> = None;

        // Find the player on the board
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y];
                if tile.state == STATE_PLAYER {
                    if tile.player == player.key.clone() {
                        player_position = Some((x, y));
                    }
                    // Printing the whole board eats too much compute, only use locally
                    // msg!("{} {}", tile.player, tile.state);
                }
            }
        }

        // If the player is on the board move him
        match player_position {
            None => {
                return Err(SevenSeasError::TriedToMovePlayerThatWasNotOnTheBoard.into());
            }
            Some(val) => {
                let mut new_player_position: (usize, usize) = (val.0, val.1);
                match direction {
                    // Up
                    0 => {
                        if new_player_position.1 == 0 {
                            new_player_position.1 = BOARD_SIZE_Y - 1;
                        } else {
                            new_player_position.1 -= 1;
                        }
                    }
                    // Right
                    1 => {
                        if new_player_position.0 == BOARD_SIZE_X - 1 {
                            new_player_position.0 = 0;
                        } else {
                            new_player_position.0 += 1;
                        }
                    }
                    // Down
                    2 => {
                        if new_player_position.1 == BOARD_SIZE_Y - 1 {
                            new_player_position.1 = 0;
                        } else {
                            new_player_position.1 += 1;
                        }
                    }
                    // Left
                    3 => {
                        if new_player_position.0 == 0 {
                            new_player_position.0 = BOARD_SIZE_X - 1;
                        } else {
                            new_player_position.0 -= 1;
                        }
                    }
                    _ => {
                        return Err(SevenSeasError::WrongDirectionInput.into());
                    }
                }

                let new_tile = self.board[new_player_position.0][new_player_position.1];
                if new_tile.state == STATE_EMPTY {
                    self.board[new_player_position.0][new_player_position.1] =
                        self.board[player_position.unwrap().0][player_position.unwrap().1];
                    self.board[player_position.unwrap().0][player_position.unwrap().1].state =
                        STATE_EMPTY;
                    self.board[new_player_position.0][new_player_position.1].look_direction =
                        direction;

                    msg!("Moved player to new tile");
                } else {
                    msg!(
                        "player position {} {}",
                        player_position.unwrap().0,
                        player_position.unwrap().1
                    );
                    msg!(
                        "new player position {} {}",
                        new_player_position.0,
                        new_player_position.1
                    );
                    if new_tile.state == STATE_CHEST {
                        self.board[new_player_position.0][new_player_position.1] =
                            self.board[player_position.unwrap().0][player_position.unwrap().1];
                        self.board[player_position.unwrap().0][player_position.unwrap().1].state =
                            STATE_EMPTY;
                        **chest_vault.try_borrow_mut_lamports()? -= new_tile.collect_reward;
                        **player.try_borrow_mut_lamports()? += new_tile.collect_reward;
                        let transfer_instruction = Transfer {
                            from: vault_token_account,
                            to: player_token_account,
                            authority: token_account_owner_pda,
                        };

                        let seeds = &[b"token_account_owner_pda".as_ref(), &[token_owner_bump]];
                        let signer = &[&seeds[..]];

                        let cpi_ctx = CpiContext::new_with_signer(
                            token_program,
                            transfer_instruction,
                            signer,
                        );
                        anchor_spl::token::transfer(
                            cpi_ctx,
                            CHEST_COIN_REWARD * TOKEN_DECIMAL_MULTIPLIER,
                        )?;

                        let item = GameAction {
                            action_id: self.action_id,
                            action_type: GAME_ACTION_SHIP_COINS_COLLECTED,
                            player: player.key(),
                            target: player.key(),
                            damage: CHEST_COIN_REWARD,
                        };
                        self.add_new_game_action(game_actions, item);

                        msg!("Collected Chest");
                    } else if new_tile.state == STATE_PLAYER {
                        self.attack_tile(
                            (new_player_position.0, new_player_position.1),
                            1,
                            player.clone(),
                            chest_vault.clone(),
                            game_actions,
                            &vault_token_account,
                            &player_token_account,
                            &token_account_owner_pda,
                            &token_program,
                            token_owner_bump,
                        )?;

                        msg!("Other player killed");
                    }

                    msg!("{} type {}", new_tile.player, new_tile.state);
                }
            }
        }

        Ok(())
    }

    pub fn clear(&mut self) -> Result<()> {
        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                self.board[x][y].state = STATE_EMPTY;
            }
        }
        Ok(())
    }

    pub fn spawn_player(
        &mut self,
        player: AccountInfo,
        avatar: Pubkey,
        ship: &mut Ship,
        extra_health: u64,
    ) -> Result<()> {
        let mut empty_slots: Vec<(usize, usize)> = Vec::new();

        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y];
                if tile.state == STATE_EMPTY {
                    empty_slots.push((x, y));
                } else {
                    if tile.player == player.key.clone() && tile.state == STATE_PLAYER {
                        return Err(SevenSeasError::PlayerAlreadyExists.into());
                    }
                    //msg!("{}", tile.player);
                }
            }
        }

        if empty_slots.len() == 0 {
            return Err(SevenSeasError::BoardIsFull.into());
        }

        let mut rng = XorShift64 {
            a: empty_slots.len() as u64,
        };

        let random_empty_slot = empty_slots[(rng.next() % (empty_slots.len() as u64)) as usize];
        msg!(
            "Player spawn at {} {}",
            random_empty_slot.0,
            random_empty_slot.1
        );

        let mut range: u16 = 1;

        match ship.upgrades {
            0 | 1 | 2 => {
                range = 1;
            }
            _ => {
                range = 2;
            }
        }

        self.board[random_empty_slot.0][random_empty_slot.1] = Tile {
            player: player.key.clone(),
            avatar: avatar.clone(),
            state: STATE_PLAYER,
            health: ship.health + extra_health,
            start_health: ship.health + extra_health,
            damage: ship.cannons,
            range: range,
            collect_reward: PLAYER_KILL_REWARD,
            look_direction: 0,
            ship_level: ship.upgrades,
        };

        Ok(())
    }

    pub fn spawn_chest(&mut self, player: AccountInfo) -> Result<()> {
        let mut empty_slots: Vec<(usize, usize)> = Vec::new();

        for x in 0..BOARD_SIZE_X {
            for y in 0..BOARD_SIZE_Y {
                let tile = self.board[x][y];
                if tile.state == STATE_EMPTY {
                    empty_slots.push((x, y));
                } else {
                    //msg!("{}", tile.player);
                }
            }
        }

        if empty_slots.len() == 0 {
            return Err(SevenSeasError::BoardIsFull.into());
        }

        let mut rng = XorShift64 {
            a: (empty_slots.len() + 1) as u64,
        };

        let random_empty_slot = empty_slots[(rng.next() % (empty_slots.len() as u64)) as usize];
        msg!(
            "Chest spawn at {} {}",
            random_empty_slot.0,
            random_empty_slot.1
        );

        self.board[random_empty_slot.0][random_empty_slot.1] = Tile {
            player: player.key.clone(),
            avatar: player.key.clone(),
            state: STATE_CHEST,
            health: 1,
            start_health: 1,
            damage: 0,
            range: 0,
            collect_reward: CHEST_REWARD,
            look_direction: 0,
            ship_level: 0,
        };

        Ok(())
    }
}

#[account]
pub struct ChestVaultAccount {}

pub struct XorShift64 {
    a: u64,
}

impl XorShift64 {
    pub fn next(&mut self) -> u64 {
        let mut x = self.a;
        x ^= x << 13;
        x ^= x >> 7;
        x ^= x << 17;
        self.a = x;
        x
    }
}
