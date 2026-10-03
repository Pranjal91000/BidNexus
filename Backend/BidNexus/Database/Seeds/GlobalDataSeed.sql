-- BidNexus GlobalData Seed
-- Auction lifecycle status reference data.
--
-- Finalized Auction lifecycle:
--   Draft -> Authorized -> Open for Intent -> Intent Evaluation -> Scheduled -> Open -> Completed
--
-- Open-to-all auctions skip the intent states:
--   Draft -> Authorized -> Scheduled -> Open -> Completed
--
-- IDs are explicit and stable because Auction.StatusId is a foreign key
-- to GlobalData.Status.Id.
--
-- This script intentionally seeds ONLY Status. Other GlobalData reference
-- values will be added after their domain semantics are finalized.

BEGIN;

INSERT INTO "GlobalData"."Status" ("Id", "Name", "Inactive")
VALUES
    (1, 'Draft',             FALSE),
    (2, 'Authorized',        FALSE),
    (3, 'Open for Intent',   FALSE),
    (4, 'Intent Evaluation', FALSE),
    (5, 'Scheduled',         FALSE),
    (6, 'Open',              FALSE),
    (7, 'Completed',         FALSE)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Inactive" = EXCLUDED."Inactive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."Status"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."Status"), 1),
    TRUE
);

COMMIT;
