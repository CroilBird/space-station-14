# base ui elements
parrot-messages-title = Parrot messages

# base controls
parrot-messages-loading = Loading parrot messages...
parrot-messages-num-messages = { $messageCount -> 
    [0] No active parrot messages.
    [1] 1 active parrot message.
    *[other] { $messageCount } active parrot messages.
}
parrot-messages-show-blocked = Show blocked messages
parrot-messages-show-blocked-tooltip = Blocked messages are never picked by entities that use the parrot message database and will not be re-learned.
parrot-messages-show-old = Show old messages
parrot-messages-show-old-tooltip = Old messages are messages older than the parrot message age cutoff set using the relevant Cvar.
parrot-messages-refresh = Refresh
parrot-messages-apply-filter = Apply filter
parrot-messages-clear-filter = Clear filter

# message line elements
parrot-messages-line-ahelp-tooltip = Ahelp this user. If this button is greyed out, this user is not in the current round or online.
parrot-messages-line-block = Block
parrot-messages-line-block-tooltip = Block this message, preventing it from being picked by entities using the parrot message database and preventing it from being learnt.
parrot-messages-line-unblock = Unblock
parrot-messages-line-unblock-tooltip = Unblock this message. If there is a Cvar set to discard old messages, this message may be discarded. Otherwise, this message can again be picked by entities using the parrot message database.
