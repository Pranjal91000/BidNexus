-- ==============================================================================
-- BidNexus - Global Data Seed Script
-- Target Database: PostgreSQL
-- Schema: GlobalData
--
-- This script populates all global reference / lookup data required by the
-- BidNexus application. It is fully idempotent (safe to run multiple times)
-- using ON CONFLICT clauses, and updates the identity sequences upon completion.
-- ==============================================================================

BEGIN;

CREATE SCHEMA IF NOT EXISTS "GlobalData";

-- ------------------------------------------------------------------------------
-- 1. Status (StatusEnum)
-- ------------------------------------------------------------------------------
-- Mappings match Core.Enumeration.StatusEnum:
-- 1 = Draft
-- 2 = Authorized
-- 3 = OpenForIntent
-- 4 = IntentEvaluation
-- 5 = Scheduled
-- 6 = Open
-- 7 = Completed
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."Status" ("Id", "Name", "Inactive")
VALUES
    (1, 'Draft', false),
    (2, 'Authorized', false),
    (3, 'OpenForIntent', false),
    (4, 'IntentEvaluation', false),
    (5, 'Scheduled', false),
    (6, 'Open', false),
    (7, 'Completed', false)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Inactive" = EXCLUDED."Inactive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."Status"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."Status"), 1)
);

-- ------------------------------------------------------------------------------
-- 2. ChargeType (ChargeTypeEnum)
-- ------------------------------------------------------------------------------
-- Mappings match Core.Enumeration.ChargeTypeEnum:
-- 1 = Fixed (FIXED)
-- 2 = Percentage (PERCENTAGE)
-- 3 = Per Unit (PER_UNIT)
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."ChargeType" ("Id", "Name", "Code", "IsActive")
VALUES
    (1, 'Fixed Amount', 'FIXED', true),
    (2, 'Percentage', 'PERCENTAGE', true),
    (3, 'Per Unit', 'PER_UNIT', true)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Code" = EXCLUDED."Code",
    "IsActive" = EXCLUDED."IsActive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."ChargeType"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."ChargeType"), 1)
);

-- ------------------------------------------------------------------------------
-- 3. TaxNature (TaxNatureEnum)
-- ------------------------------------------------------------------------------
-- Mappings match Core.Enumeration.TaxNatureEnum:
-- 1 = Additive (ADDITIVE)
-- 2 = Deductive (DEDUCTIVE)
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."TaxNature" ("Id", "Name", "Code", "IsActive")
VALUES
    (1, 'Additive', 'ADDITIVE', true),
    (2, 'Deductive', 'DEDUCTIVE', true)
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Code" = EXCLUDED."Code",
    "IsActive" = EXCLUDED."IsActive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."TaxNature"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."TaxNature"), 1)
);

-- ------------------------------------------------------------------------------
-- 4. Category
-- ------------------------------------------------------------------------------
-- Procurement item categories used across Master Items and Auctions.
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."Category" ("Id", "Name", "Code")
VALUES
    (1, 'General Procurement', 'GEN'),
    (2, 'Raw Materials & Metals', 'RAW'),
    (3, 'Machinery & Equipment', 'EQP'),
    (4, 'Services & Operations', 'SVC'),
    (5, 'IT & Software Solutions', 'IT'),
    (6, 'Logistics & Transportation', 'LOG'),
    (7, 'Electrical & Electronics', 'ELE'),
    (8, 'Chemicals & Petrochemicals', 'CHEM'),
    (9, 'Office Supplies & Facilities', 'OFF'),
    (10, 'Construction & Infrastructure', 'CONST')
ON CONFLICT ("Id") DO UPDATE
SET
    "Name" = EXCLUDED."Name",
    "Code" = EXCLUDED."Code";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."Category"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."Category"), 1)
);

-- ------------------------------------------------------------------------------
-- 5. RatingFor
-- ------------------------------------------------------------------------------
-- Target roles for post-auction evaluations:
-- 1 = Vendor (rated by Organization / Buyer)
-- 2 = Organization (rated by Vendor / Winning Bidder)
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."RatingFor" ("Id", "For", "Inactive")
VALUES
    (1, 'Vendor', false),
    (2, 'Organization', false)
ON CONFLICT ("Id") DO UPDATE
SET
    "For" = EXCLUDED."For",
    "Inactive" = EXCLUDED."Inactive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."RatingFor"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."RatingFor"), 1)
);

-- ------------------------------------------------------------------------------
-- 6. RatingParameter
-- ------------------------------------------------------------------------------
-- Metric breakdown parameters evaluated for each target role.
-- ------------------------------------------------------------------------------
INSERT INTO "GlobalData"."RatingParameter" ("Id", "ParameterName", "RatingFor", "Inactive")
VALUES
    -- Vendor Rating Parameters (RatingFor = true)
    (1, 'Quality & Specification Compliance', true, false),
    (2, 'Delivery & Schedule Adherence', true, false),
    (3, 'Commercial & Pricing Competitiveness', true, false),
    (4, 'Communication & Responsiveness', true, false),
    (5, 'Packaging & Documentation', true, false),

    -- Organization Rating Parameters (RatingFor = false)
    (6, 'Payment Promptness & Terms Adherence', false, false),
    (7, 'Clarity of Requirements & Scope', false, false),
    (8, 'Process Fairness & Transparency', false, false),
    (9, 'Coordination & Operational Support', false, false),
    (10, 'Dispute Resolution & Professionalism', false, false)
ON CONFLICT ("Id") DO UPDATE
SET
    "ParameterName" = EXCLUDED."ParameterName",
    "RatingFor" = EXCLUDED."RatingFor",
    "Inactive" = EXCLUDED."Inactive";

SELECT setval(
    pg_get_serial_sequence('"GlobalData"."RatingParameter"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "GlobalData"."RatingParameter"), 1)
);

COMMIT;
