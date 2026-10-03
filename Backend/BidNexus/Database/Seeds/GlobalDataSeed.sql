-- BidNexus GlobalData Seed
-- Auction lifecycle status reference data.
-- ChargeType and TaxNature reference data.
--
-- Finalized Auction lifecycle:
--   Draft -> Authorized -> Open for Intent -> Intent Evaluation -> Scheduled -> Open -> Completed
--
-- Open-to-all auctions skip the intent states:
--   Draft -> Authorized -> Scheduled -> Open -> Completed
--
-- IDs are explicit and stable for reference-data foreign keys.

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

INSERT INTO "GlobalData"."ChargeType" ("Id", "Name", "Code", "IsActive")
VALUES
    (1, 'Fixed',      'FIXED',      TRUE),
    (2, 'Percentage', 'PERCENTAGE', TRUE)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Code" = EXCLUDED."Code",
    "IsActive" = EXCLUDED."IsActive";

INSERT INTO "GlobalData"."TaxNature" ("Id", "Name", "Code", "IsActive")
VALUES
    (1, 'Additive',  'ADDITIVE',  TRUE),
    (2, 'Deductive', 'DEDUCTIVE', TRUE)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Code" = EXCLUDED."Code",
    "IsActive" = EXCLUDED."IsActive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."Status"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."Status"), 1),
    TRUE
);

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."ChargeType"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."ChargeType"), 1),
    TRUE
);

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."TaxNature"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."TaxNature"), 1),
    TRUE
);

COMMIT;
