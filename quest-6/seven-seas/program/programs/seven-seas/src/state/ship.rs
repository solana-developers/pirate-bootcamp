use anchor_lang::prelude::*;

#[account]
pub struct Ship {
    pub health: u64,
    pub kills: u16,
    pub cannons: u64,
    pub upgrades: u16,
    pub xp: u16,
    pub level: u16,
    pub start_health: u64,
}