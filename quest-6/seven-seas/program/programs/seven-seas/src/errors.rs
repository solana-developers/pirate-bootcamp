use anchor_lang::error_code;

#[error_code]
pub enum SevenSeasError {
    TileOutOfBounds,
    BoardIsFull,
    PlayerAlreadyExists,
    TriedToMovePlayerThatWasNotOnTheBoard,
    TriedToShootWithPlayerThatWasNotOnTheBoard,
    WrongDirectionInput,
    MaxShipLevelReached,
    CouldNotFindAShipToAttack,

}
