-- ============================================================================
-- BidNexus Comprehensive Dummy Data Seed Script
-- Organizations (12), Vendors (30), Master Items (24), Attachments, and Auctions
-- Password for all seeded users: Password@123
-- Hash: AQAAAAIAAYagAAAAEEByu9SWIB2RwiKr/egy/9blWlyJYu+SzAtIGGI0UJI+rLM14gyK4jifKyw0dCrAKw==
-- ============================================================================

-- 1. Seed Organizations (12 Enterprise Buyers)
DO $$
DECLARE
    v_pwd text := 'AQAAAAIAAYagAAAAEEByu9SWIB2RwiKr/egy/9blWlyJYu+SzAtIGGI0UJI+rLM14gyK4jifKyw0dCrAKw==';
    v_tid integer;
    v_oid integer;
BEGIN
    -- Org 1
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'tatasteel') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Tata Steel Industrial Corp', '+91 22 6665 8282', 'procurement@tatasteel.com', 'tatasteel', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Tata Steel Industrial Corp', 'Bombay House, 24 Homi Mody Street, Fort, Mumbai - 400001', 'Global diversified steel manufacturer providing advanced structural steel, flat products, and specialized infrastructure engineering solutions.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 2
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'relianceinfra') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Reliance Infrastructure Ltd', '+91 22 4303 1000', 'tenders@relianceinfra.com', 'relianceinfra', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Reliance Infrastructure Ltd', 'Reliance Centre, 19 Walchand Hirachand Marg, Ballard Estate, Mumbai - 400001', 'Premier infrastructure development enterprise delivering mega road corridors, metro rail transport, power transmission networks, and EPC projects across India.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 3
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'lntheavy') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Larsen & Toubro Heavy Engineering', '+91 22 6752 5656', 'contracts@lntecc.com', 'lntheavy', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Larsen & Toubro Heavy Engineering', 'L&T House, Ballard Estate, Mumbai - 400001', 'Globally acknowledged engineering giant manufacturing critical equipment and custom systems for refinery, petrochemical, nuclear power, and aerospace installations.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 4
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'adanienergy') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Adani Energy Solutions', '+91 79 2656 5555', 'bids@adanienergy.com', 'adanienergy', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Adani Energy Solutions', 'Adani Corporate House, Shantigram, S.G. Highway, Ahmedabad - 382421', 'Pioneering private transmission and utility company building resilient electrical grid networks, smart metering ecosystems, and sustainable power infrastructure.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 5
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'bhel') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Bharat Heavy Electricals Ltd', '+91 11 6633 7000', 'epc@bhel.in', 'bhel', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Bharat Heavy Electricals Ltd', 'BHEL House, Siri Fort, New Delhi - 110049', 'Leading public sector engineering enterprise specializing in power equipment, gas turbines, traction motors, and heavy industrial automation solutions.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 6
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'jswsteel') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('JSW Steel Ltd', '+91 22 4286 1000', 'sourcing@jsw.in', 'jswsteel', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('JSW Steel Ltd', 'JSW Centre, Bandra Kurla Complex, Bandra East, Mumbai - 400051', 'Indias prominent integrated steel company with world-class manufacturing plants in Vijayanagar, Dolvi, and Salem providing high-performance alloy products.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 7
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'hpcl') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Hindustan Petroleum Corp Ltd', '+91 22 2286 3900', 'contracts@hpcl.in', 'hpcl', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Hindustan Petroleum Corp Ltd', 'Petroleum House, 17 Jamshedji Tata Road, Churchgate, Mumbai - 400020', 'Navratna mega energy corporation engaged in petroleum refining, fuel pipeline networks, petrochemical synthesis, and nationwide retail fuel distribution.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 8
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vedanta') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Vedanta Resources Ltd', '+91 22 6646 1000', 'commercial@vedanta.co.in', 'vedanta', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Vedanta Resources Ltd', 'Core-6, Scope Complex, Lodhi Road, New Delhi - 110003', 'Natural resources conglomerate with globally ranked operations in zinc, lead, silver, aluminum, copper, iron ore, and oil & gas extraction.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 9
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'iocl') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Indian Oil Corporation Ltd', '+91 11 2436 0151', 'materials@indianoil.in', 'iocl', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Indian Oil Corporation Ltd', 'IndianOil Bhavan, 1 Sri Aurobindo Marg, Yusuf Sarai, New Delhi - 110016', 'Highest-ranked Indian energy PSU fueling the nation with state-of-the-art refining capabilities, cryogenic gas logistics, and petrochemical complexes.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 10
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'mahindra') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Mahindra Heavy Engineering', '+91 22 2490 1441', 'procure@mahindra.com', 'mahindra', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Mahindra Heavy Engineering', 'Mahindra Towers, G.M. Bhosale Marg, Worli, Mumbai - 400018', 'Precision engineering, automotive assembly systems, and defense equipment manufacturing arm of the Mahindra Group.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 11
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'ultratech') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('UltraTech Cement Ltd', '+91 22 6691 7800', 'tenders@ultratechcement.com', 'ultratech', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('UltraTech Cement Ltd', 'Ahura Centre, B-Wing, Mahakali Caves Road, Andheri East, Mumbai - 400093', 'Largest manufacturer of grey cement, ready-mix concrete, and white cement in India with extensive global export footprints.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;

    -- Org 12
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'siemensenergy') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Siemens Energy India', '+91 22 3967 7000', 'projects@siemensenergy.com', 'siemensenergy', v_pwd, 0, false, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Organization" ("Name", "OfficialAddress", "About", "TenantId")
        VALUES ('Siemens Energy India', 'Birla Aurora, Level 21, Dr. Annie Besant Road, Worli, Mumbai - 400030', 'Global technology leader driving decarbonization, gas and steam turbine systems, offshore wind grid connections, and grid transmission technologies.', v_tid)
        RETURNING "Id" INTO v_oid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_oid WHERE "Id" = v_tid;
    END IF;
END $$;

-- 2. Seed Vendors (30 Qualified Suppliers)
DO $$
DECLARE
    v_pwd text := 'AQAAAAIAAYagAAAAEEByu9SWIB2RwiKr/egy/9blWlyJYu+SzAtIGGI0UJI+rLM14gyK4jifKyw0dCrAKw==';
    v_tid integer;
    v_vid integer;
BEGIN
    -- Vendor 1
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_apexvalves') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Apex Industrial Valves & Controls', '+91 22 2850 1122', 'sales@apexvalves.com', 'vendor_apexvalves', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Apex Industrial Valves & Controls', 'ISO 9001 certified manufacturer of trunnion ball valves, butterfly valves, and high-pressure steam check valves.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 2
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_apexsteel') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Apex Steel Fabricators & Forgings', '+91 11 2541 3344', 'info@apexsteelfab.com', 'vendor_apexsteel', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Apex Steel Fabricators & Forgings', 'Manufacturer of custom structural steel frames, heavy weldments, and hot-rolled alloy forging components.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 3
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_bharattrf') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Bharat Electrical Transformers', '+91 79 2583 4455', 'orders@bharattrf.in', 'vendor_bharattrf', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Bharat Electrical Transformers', 'Specialist producer of oil-immersed power transformers, dry-type distribution transformers, and substation switchgear.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 4
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_zenithhyd') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Zenith Heavy Hydraulics Ltd', '+91 44 2625 5566', 'contact@zenithhydraulics.com', 'vendor_zenithhyd', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Zenith Heavy Hydraulics Ltd', 'Engineering hydraulic power units, variable displacement axial pumps, high-pressure cylinders, and manifold assemblies.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 5
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_precfast') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Precision Fasteners & Bolts', '+91 161 253 6677', 'bids@precisionfasteners.co', 'vendor_precfast', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Precision Fasteners & Bolts', 'High tensile Grade 8.8, 10.9, and 12.9 metric bolts, anchor studs, heavy hex nuts, and stainless steel fasteners.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 6
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_titalloy') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Titanium & Special Alloys Corp', '+91 22 2385 7788', 'sales@titalloy.com', 'vendor_titalloy', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Titanium & Special Alloys Corp', 'Stockist and supplier of Inconel, Monel, Hastelloy, Duplex 2205, and titanium sheets, bars, and pipe fittings.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 7
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_vanguardcrn') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Vanguard Industrial Cranes & Hoists', '+91 20 2712 8899', 'crane.sales@vanguard.in', 'vendor_vanguardcrn', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Vanguard Industrial Cranes & Hoists', 'Design, erection, and commissioning of double girder EOT cranes, gantry cranes, and wire rope electric hoists.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 8
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_omegabrgs') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Omega Mechanical Bearings Co', '+91 33 2230 9900', 'info@omegabearings.com', 'vendor_omegabrgs', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Omega Mechanical Bearings Co', 'Authorized distributor and manufacturer of spherical roller bearings, deep groove ball bearings, and pillow blocks.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 9
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_solaris') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Solaris Renewable Panels & Inverters', '+91 80 4120 1011', 'sales@solarisrenewables.in', 'vendor_solaris', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Solaris Renewable Panels & Inverters', 'Tier-1 supplier of mono PERC bifacial solar panels, central grid-tie solar inverters, and mounting structures.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 10
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_pioneerfire') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Pioneer Fire Safety & Deluge Systems', '+91 22 2778 2022', 'solutions@pioneerfire.com', 'vendor_pioneerfire', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Pioneer Fire Safety & Deluge Systems', 'UL/FM certified deluge valves, deluge foam skid units, sprinkler heads, and high-velocity water spray systems.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 11
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_conticonv') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Continental Conveyors & Belting', '+91 40 2780 3033', 'belts@continentalconv.com', 'vendor_conticonv', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Continental Conveyors & Belting', 'Heavy duty steel cord conveyor belts, chevron heat-resistant belting, idler rollers, and pulley assemblies.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 12
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_technopumps') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('TechnoPumps & Fluid Systems', '+91 22 6123 4044', 'sales@technopumps.in', 'vendor_technopumps', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('TechnoPumps & Fluid Systems', 'Centrifugal end-suction, multistage boiler feed, and slurry pumps for industrial processing and mining.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 13
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_thermashield') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('ThermaShield High Temp Insulation', '+91 124 456 5055', 'quotes@thermashield.com', 'vendor_thermashield', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('ThermaShield High Temp Insulation', 'Ceramic fiber blankets, calcium silicate pipe sections, microporous boards, and thermal insulation jackets.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 14
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_electrosafe') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('ElectroSafe Flameproof Switchgears', '+91 265 233 6066', 'marketing@electrosafe.in', 'vendor_electrosafe', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('ElectroSafe Flameproof Switchgears', 'ATEX and PESO approved explosion-proof flameproof electrical panels, push button stations, and LED floodlights.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 15
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_deltamach') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Delta Heavy Machinery & Spares', '+91 22 2500 7077', 'spares@deltamachinery.com', 'vendor_deltamach', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Delta Heavy Machinery & Spares', 'Original earthmoving spare parts, excavator buckets, hydraulic rock breakers, and undercarriage components.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 16
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_indogerman') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('IndoGerman IE4 Electric Motors', '+91 20 6688 8088', 'motors@indogerman.com', 'vendor_indogerman', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('IndoGerman IE4 Electric Motors', 'Super premium efficiency IE3 & IE4 squirrel cage induction motors from 0.75kW to 355kW.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 17
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_vertexgaskets') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Vertex Industrial Gaskets & Seals', '+91 22 2834 9099', 'seals@vertexgaskets.com', 'vendor_vertexgaskets', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Vertex Industrial Gaskets & Seals', 'Spiral wound gaskets, camprofile gaskets, RTJ ring joint gaskets, and virgin PTFE sheet jointing.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 18
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_aquaclear') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Aquaclear Water Treatment Chemicals', '+91 79 2680 0110', 'chem@aquaclear.in', 'vendor_aquaclear', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Aquaclear Water Treatment Chemicals', 'Cooling tower scale inhibitors, RO antiscalants, biocides, coagulants, and boiler water oxygen scavengers.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 19
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_kirloskar') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Kirloskar Flow Technology', '+91 20 2740 1221', 'flow@kirloskarflow.in', 'vendor_kirloskar', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Kirloskar Flow Technology', 'Large split-case water transmission pumps, vertical turbine pumps, and municipal water supply solutions.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 20
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_godrej') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Godrej Material Handling Solutions', '+91 22 6796 2332', 'mhe@godrej.com', 'vendor_godrej', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Godrej Material Handling Solutions', 'Electric counterbalance forklifts, reach trucks, pallet stackers, and automated guided vehicles (AGVs).', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 21
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_polycab') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Polycab Industrial Power Cables', '+91 22 2432 3443', 'industrial@polycab.com', 'vendor_polycab', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Polycab Industrial Power Cables', 'HT and LT XLPE armored power cables, control cables, instrumentation cables, and solar DC cables.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 22
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_supremepetro') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Supreme Petrochem Industrial Additives', '+91 22 6709 4554', 'polymers@supremepetro.com', 'vendor_supremepetro', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Supreme Petrochem Industrial Additives', 'Polystyrene grades, EPS beads, masterbatches, and specialty compounding resin raw materials.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 23
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_havells') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Havells Heavy Duty Switchgears', '+91 120 477 5665', 'switchgear@havells.com', 'vendor_havells', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Havells Heavy Duty Switchgears', 'Air circuit breakers (ACB), molded case circuit breakers (MCCB), changeover switches, and industrial distribution boards.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 24
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_thermax') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Thermax Energy & Boilers', '+91 20 6605 6776', 'energy@thermaxglobal.com', 'vendor_thermax', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Thermax Energy & Boilers', 'Packaged steam boilers, thermal oil heaters, waste heat recovery units, and electrostatic precipitators.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 25
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_carborundum') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Carborundum Universal Abrasives', '+91 44 3000 7887', 'abrasives@cumi.murugappa.com', 'vendor_carborundum', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Carborundum Universal Abrasives', 'Bonded abrasives, coated abrasives, diamond cutting wheels, and wear-resistant silicon carbide ceramics.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 26
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_skf') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('SKF Bearings & Condition Monitoring', '+91 20 6611 8998', 'industrial.sales@skf.com', 'vendor_skf', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('SKF Bearings & Condition Monitoring', 'Precision bearings, lubrication systems, vibration analysis sensors, and predictive maintenance solutions.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 27
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_crompton') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Crompton Industrial Drives', '+91 22 2423 9109', 'drives@cgglobal.com', 'vendor_crompton', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Crompton Industrial Drives', 'Low and medium voltage variable frequency drives (VFD), soft starters, and industrial automation panels.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 28
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_schneider') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Schneider Automation Systems', '+91 124 394 0210', 'automation@se.com', 'vendor_schneider', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Schneider Automation Systems', 'Modicon PLCs, SCADA software, human-machine interfaces (HMI), and smart power distribution systems.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 29
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_abb') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('ABB Heavy Power Grid Solutions', '+91 80 2294 1321', 'powergrids@abb.com', 'vendor_abb', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('ABB Heavy Power Grid Solutions', 'High voltage gas-insulated switchgear (GIS), circuit breakers, instrument transformers, and grid protection relays.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;

    -- Vendor 30
    IF NOT EXISTS (SELECT 1 FROM "TenantRel"."Tenant" WHERE "UserName" = 'vendor_linde') THEN
        INSERT INTO "TenantRel"."Tenant" ("Name", "ContactNumber", "EmailAddress", "UserName", "Password", "ReferenceId", "IsVendor", "IsBlocked")
        VALUES ('Linde Industrial Gases & Welding', '+91 33 6602 2432', 'gases@linde.com', 'vendor_linde', v_pwd, 0, true, false)
        RETURNING "Id" INTO v_tid;
        INSERT INTO "TenantRel"."Vendor" ("Name", "About", "TenantId")
        VALUES ('Linde Industrial Gases & Welding', 'Industrial bulk liquid oxygen, nitrogen, argon, specialty gas mixtures, and high-performance welding equipment.', v_tid)
        RETURNING "Id" INTO v_vid;
        UPDATE "TenantRel"."Tenant" SET "ReferenceId" = v_vid WHERE "Id" = v_tid;
    END IF;
END $$;

-- 3. Seed Units if not existing
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM "Master"."Unit" WHERE "Code" = 'KG') THEN
        INSERT INTO "Master"."Unit" ("TenantId", "Name", "Code", "CreatedDateTime", "LastModifiedDateTime", "StatusId", "StatusRemarks")
        VALUES (1, 'Kilogram', 'KG', NOW(), NOW(), 1, 'Standard weight unit');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM "Master"."Unit" WHERE "Code" = 'MTR') THEN
        INSERT INTO "Master"."Unit" ("TenantId", "Name", "Code", "CreatedDateTime", "LastModifiedDateTime", "StatusId", "StatusRemarks")
        VALUES (1, 'Meter', 'MTR', NOW(), NOW(), 1, 'Length unit');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM "Master"."Unit" WHERE "Code" = 'NOS') THEN
        INSERT INTO "Master"."Unit" ("TenantId", "Name", "Code", "CreatedDateTime", "LastModifiedDateTime", "StatusId", "StatusRemarks")
        VALUES (1, 'Numbers / Pieces', 'NOS', NOW(), NOW(), 1, 'Count unit');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM "Master"."Unit" WHERE "Code" = 'MT') THEN
        INSERT INTO "Master"."Unit" ("TenantId", "Name", "Code", "CreatedDateTime", "LastModifiedDateTime", "StatusId", "StatusRemarks")
        VALUES (1, 'Metric Ton', 'MT', NOW(), NOW(), 1, 'Bulk tonnage unit');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM "Master"."Unit" WHERE "Code" = 'SET') THEN
        INSERT INTO "Master"."Unit" ("TenantId", "Name", "Code", "CreatedDateTime", "LastModifiedDateTime", "StatusId", "StatusRemarks")
        VALUES (1, 'Assembly Set', 'SET', NOW(), NOW(), 1, 'Package unit');
    END IF;
END $$;

-- 3. Seed Items with DocAttachmentId
DO $$
DECLARE
    v_unit_nos_id integer;
    v_unit_kg_id integer;
    v_unit_mt_id integer;
    v_cat_id smallint;
    v_tenant_id integer;
    v_item_id integer;
BEGIN
    SELECT "Id" INTO v_unit_nos_id FROM "Master"."Unit" WHERE "Code" = 'NOS' LIMIT 1;
    IF v_unit_nos_id IS NULL THEN SELECT "Id" INTO v_unit_nos_id FROM "Master"."Unit" LIMIT 1; END IF;
    
    SELECT "Id" INTO v_unit_kg_id FROM "Master"."Unit" WHERE "Code" = 'KG' LIMIT 1;
    IF v_unit_kg_id IS NULL THEN v_unit_kg_id := v_unit_nos_id; END IF;

    SELECT "Id" INTO v_unit_mt_id FROM "Master"."Unit" WHERE "Code" = 'MT' LIMIT 1;
    IF v_unit_mt_id IS NULL THEN v_unit_mt_id := v_unit_nos_id; END IF;

    SELECT "Id" INTO v_cat_id FROM "GlobalData"."Category" LIMIT 1;
    IF v_cat_id IS NULL THEN v_cat_id := 1; END IF;

    SELECT "Id" INTO v_tenant_id FROM "TenantRel"."Tenant" WHERE "IsVendor" = false ORDER BY "Id" ASC LIMIT 1;
    IF v_tenant_id IS NULL THEN v_tenant_id := 1; END IF;

    -- Helper macro-like inserts for items
    -- Item 1: Steel Plate
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'STL-PLT-10') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Structural Steel Plate 10mm IS 2062', 'STL-PLT-10', v_cat_id, 'Hot-rolled structural steel plate, 10mm thickness, Grade E250/E350 with ultrasonic testing certification.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_mt_id);
    END IF;

    -- Item 2: Steel Pipe
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'STL-PIP-150') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Seamless Carbon Steel Pipe 6 Inch Sch 40', 'STL-PIP-150', v_cat_id, 'ASTM A106 Grade B seamless steel pipe, 150mm NB, bevelled ends, hydro-tested to 150 bar.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 3: Ball Valve
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'VLV-BAL-100') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Class 300 Trunnion Ball Valve 4 Inch', 'VLV-BAL-100', v_cat_id, 'API 6D flanged ball valve, A216 WCB body, SS316 ball & stem, fire-safe graphite packing.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 4: Hydraulic Pump
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'PMP-HYD-50') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Variable Displacement Axial Piston Pump', 'PMP-HYD-50', v_cat_id, 'Heavy-duty hydraulic pump, 50cc displacement, 350 bar continuous operating pressure.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 5: Generator
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'GEN-DSL-500') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '500 kVA Silent Diesel Generator Set', 'GEN-DSL-500', v_cat_id, 'Cummins powered turbo-charged diesel generator with acoustic canopy and AMF synchronization panel.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 6: Transformer
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'TRF-OIL-1000') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '1000 kVA 11kV/433V Oil Cooled Transformer', 'TRF-OIL-1000', v_cat_id, 'Copper wound distribution transformer, ONAN cooling, with Buchholz relay and OLTC.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 7: Safety Helmet
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'SFT-HLM-CE') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Industrial Safety Helmet Class E', 'SFT-HLM-CE', v_cat_id, 'High-density polyethylene shell with 6-point textile suspension, dielectric rated up to 20,000V.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 8: Weld Neck Flange
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'FLG-WNR-200') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Weld Neck Flange 8 Inch Class 150 RF', 'FLG-WNR-200', v_cat_id, 'Forged carbon steel ASTM A105 weld neck flange, raised face serrated finish to ASME B16.5.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 9: Power Cable
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'CBL-COP-400') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '3.5 Core 400 sq mm XLPE Armored Cable', 'CBL-COP-400', v_cat_id, 'Stranded compacted aluminum/copper conductor, cross-linked polyethylene insulated, galvanized steel wire armored.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 10: Solar Panel
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'SOL-PNL-550') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '550W Mono PERC Bifacial Solar PV Panel', 'SOL-PNL-550', v_cat_id, 'High efficiency half-cut monocrystalline cell module, IP68 junction box, 1500V system voltage.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 11: High Tensile Fasteners
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'FST-BLT-M24') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'High Tensile Hex Bolt M24 x 120mm Gr 10.9', 'FST-BLT-M24', v_cat_id, 'Hot-dip galvanized high-tensile structural bolts complete with 2H heavy hex nuts and hardened washers.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_kg_id);
    END IF;

    -- Item 12: Roller Bearing
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'BRG-ROL-222') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Spherical Roller Bearing 22220-E1-XL', 'BRG-ROL-222', v_cat_id, 'Self-aligning double-row roller bearing with brass cage, tapered bore, and dynamic load rating 450 kN.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 13: Pneumatic Actuator
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'ACT-PNE-160') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Rack & Pinion Double Acting Pneumatic Actuator', 'ACT-PNE-160', v_cat_id, 'Hard anodized aluminum cylinder body, NAMUR solenoid interface, output torque 500 Nm at 6 bar.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 14: Deluge Valve
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'VLV-DEL-150') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Automatic Deluge Valve 6 Inch UL/FM', 'VLV-DEL-150', v_cat_id, 'Diaphragm operated quick opening deluge valve for transformer bay water spray systems.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 15: Heavy I-Beam
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'STL-IBM-400') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Universal Heavy Structural Beam ISMB 400', 'STL-IBM-400', v_cat_id, 'Heavy I-beam section, 400mm web height x 140mm flange width, 61.6 kg/m, Grade E350.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_mt_id);
    END IF;

    -- Item 16: Conveyor Belt
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'BLT-CNV-120') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Steel Cord Conveyor Belt ST 2000 Width 1200mm', 'BLT-CNV-120', v_cat_id, 'Heavy-duty abrasion resistant grade M24 rubber belt with longitudinal steel cords for mine loadout.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 17: Insulation Blanket
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'INS-CER-142') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Ceramic Fiber High Temp Insulation Blanket 1425C', 'INS-CER-142', v_cat_id, 'Spun zirconia stabilized ceramic fiber blanket, density 128 kg/m3, thickness 25mm for furnace linings.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 18: Flameproof Light
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'LGT-EXP-150') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Explosion Proof Flameproof LED Light 150W', 'LGT-EXP-150', v_cat_id, 'Ex d IIC T6 Gb rated cast copper-free aluminum housing with toughened borosilicate glass dome.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 19: Process Pump
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'PMP-CNT-200') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'End Suction Centrifugal Process Pump 200m3/hr', 'PMP-CNT-200', v_cat_id, 'ISO 2858 process pump with SS316 impeller, duplex stainless shaft, and API 682 cartridge mechanical seal.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 20: Induction Motor
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'MOT-IND-110') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '110 kW 4-Pole Foot Mounted Induction Motor IE4', 'MOT-IND-110', v_cat_id, 'Super premium efficiency squirrel cage motor, 415V, 50Hz, IP55 enclosure with Class H insulation.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 21: Overhead Crane
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'CRN-EOT-20T') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, '20 Ton Double Girder Electric Overhead Crane', 'CRN-EOT-20T', v_cat_id, 'Span 24 meters, height of lift 12m, Class IV heavy duty with VFD speed control on all motions.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 22: Check Valve
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'VLV-CHK-250') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Dual Plate Wafer Check Valve 10 Inch Class 150', 'VLV-CHK-250', v_cat_id, 'Compact wafer body, ductile iron ASTM A536 body, Inconel spring assisted dual disc closure.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 23: Metallic Gasket
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'GSK-SPW-300') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Spiral Wound Metallic Gasket 12 Inch 300# RF', 'GSK-SPW-300', v_cat_id, 'SS316 winding with flexible expanded graphite filler and carbon steel outer centering ring.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

    -- Item 24: Chemical Scale Inhibitor
    IF NOT EXISTS (SELECT 1 FROM "Master"."Item" WHERE "Code" = 'CHM-CRN-500') THEN
        INSERT INTO "Master"."Item" ("TenantId", "Name", "Code", "CategoryId", "ItemDescription", "DocAttachmentId", "StatusId", "StatusRemarks", "CreatedDateTime", "LastModifiedDateTime")
        VALUES (v_tenant_id, 'Polymer High Molecular Weight Scale Inhibitor', 'CHM-CRN-500', v_cat_id, '200 kg drum packaging, highly effective threshold inhibitor for industrial cooling tower circuits.', NULL, 1, 'Approved for procurement', NOW(), NOW())
        RETURNING "Id" INTO v_item_id;
        INSERT INTO "Master"."ItemUnitMapping" ("TenantId", "ItemId", "UnitId") VALUES (v_tenant_id, v_item_id, v_unit_nos_id);
    END IF;

END $$;

-- 4. Create Sample Auctions, Requirements, Vendor Intents, Bids, and Statements
DO $$
DECLARE
    -- Statuses
    v_status_open smallint;
    v_status_scheduled smallint;
    v_status_completed smallint;

    -- Organizations
    v_org_tata integer;
    v_t_tata integer;
    v_org_rel integer;
    v_t_rel integer;
    v_org_lnt integer;
    v_t_lnt integer;
    v_org_adani integer;
    v_t_adani integer;
    v_org_bhel integer;
    v_t_bhel integer;
    v_org_iocl integer;
    v_t_iocl integer;

    -- Vendors
    v_v_apexvalves integer;
    v_t_apexvalves integer;
    v_v_apexsteel integer;
    v_t_apexsteel integer;
    v_v_bharattrf integer;
    v_t_bharattrf integer;
    v_v_titalloy integer;
    v_t_titalloy integer;
    v_v_pioneerfire integer;
    v_t_pioneerfire integer;
    v_v_technopumps integer;
    v_t_technopumps integer;
    v_v_deltamach integer;
    v_t_deltamach integer;
    v_v_indogerman integer;
    v_t_indogerman integer;
    v_v_polycab integer;
    v_t_polycab integer;
    v_v_havells integer;
    v_t_havells integer;
    v_v_crompton integer;
    v_t_crompton integer;
    v_v_abb integer;
    v_t_abb integer;

    -- Items & Units
    v_item_stl_plt integer;
    v_item_vlv_bal integer;
    v_item_gen_dsl integer;
    v_item_trf_oil integer;
    v_item_cbl_cop integer;
    v_unit_mt integer;
    v_unit_nos integer;
    v_unit_mtr integer;
    v_unit_set integer;

    -- Auction IDs & Requirement IDs
    v_auc1_id integer;
    v_auc1_req1_id integer;
    v_auc2_id integer;
    v_auc2_req1_id integer;
    v_auc3_id integer;
    v_auc3_req1_id integer;
    v_auc4_id integer;
    v_auc4_req1_id integer;
    v_auc5_id integer;
    v_auc5_req1_id integer;
    v_auc6_id integer;
    v_auc6_req1_id integer;

    -- Bids
    v_bid_id bigint;
    v_main_bid_id bigint;
BEGIN
    -- 1. Statuses
    SELECT "Id" INTO v_status_open FROM "GlobalData"."Status" WHERE "Name" = 'Open';
    IF v_status_open IS NULL THEN v_status_open := 6; END IF;

    SELECT "Id" INTO v_status_scheduled FROM "GlobalData"."Status" WHERE "Name" = 'Scheduled';
    IF v_status_scheduled IS NULL THEN v_status_scheduled := 5; END IF;

    SELECT "Id" INTO v_status_completed FROM "GlobalData"."Status" WHERE "Name" = 'Completed';
    IF v_status_completed IS NULL THEN v_status_completed := 7; END IF;

    -- 2. Organizations
    SELECT o."Id", o."TenantId" INTO v_org_tata, v_t_tata FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'tatasteel';
    SELECT o."Id", o."TenantId" INTO v_org_rel, v_t_rel FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'relianceinfra';
    SELECT o."Id", o."TenantId" INTO v_org_lnt, v_t_lnt FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'lntheavy';
    SELECT o."Id", o."TenantId" INTO v_org_adani, v_t_adani FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'adanienergy';
    SELECT o."Id", o."TenantId" INTO v_org_bhel, v_t_bhel FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'bhel';
    SELECT o."Id", o."TenantId" INTO v_org_iocl, v_t_iocl FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" WHERE t."UserName" = 'iocl';

    -- Fallbacks
    IF v_org_tata IS NULL THEN SELECT "Id", "TenantId" INTO v_org_tata, v_t_tata FROM "TenantRel"."Organization" ORDER BY "Id" ASC LIMIT 1; END IF;
    IF v_org_rel IS NULL THEN v_org_rel := v_org_tata; v_t_rel := v_t_tata; END IF;
    IF v_org_lnt IS NULL THEN v_org_lnt := v_org_tata; v_t_lnt := v_t_tata; END IF;
    IF v_org_adani IS NULL THEN v_org_adani := v_org_tata; v_t_adani := v_t_tata; END IF;
    IF v_org_bhel IS NULL THEN v_org_bhel := v_org_tata; v_t_bhel := v_t_tata; END IF;
    IF v_org_iocl IS NULL THEN v_org_iocl := v_org_tata; v_t_iocl := v_t_tata; END IF;

    -- 3. Vendors
    SELECT v."Id", v."TenantId" INTO v_v_apexvalves, v_t_apexvalves FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_apexvalves';
    SELECT v."Id", v."TenantId" INTO v_v_apexsteel, v_t_apexsteel FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_apexsteel';
    SELECT v."Id", v."TenantId" INTO v_v_bharattrf, v_t_bharattrf FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_bharattrf';
    SELECT v."Id", v."TenantId" INTO v_v_titalloy, v_t_titalloy FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_titalloy';
    SELECT v."Id", v."TenantId" INTO v_v_pioneerfire, v_t_pioneerfire FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_pioneerfire';
    SELECT v."Id", v."TenantId" INTO v_v_technopumps, v_t_technopumps FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_technopumps';
    SELECT v."Id", v."TenantId" INTO v_v_deltamach, v_t_deltamach FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_deltamach';
    SELECT v."Id", v."TenantId" INTO v_v_indogerman, v_t_indogerman FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_indogerman';
    SELECT v."Id", v."TenantId" INTO v_v_polycab, v_t_polycab FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_polycab';
    SELECT v."Id", v."TenantId" INTO v_v_havells, v_t_havells FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_havells';
    SELECT v."Id", v."TenantId" INTO v_v_crompton, v_t_crompton FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_crompton';
    SELECT v."Id", v."TenantId" INTO v_v_abb, v_t_abb FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" WHERE t."UserName" = 'vendor_abb';

    -- 4. Master Items & Units
    SELECT "Id" INTO v_item_stl_plt FROM "Master"."Item" WHERE "Code" = 'STL-PLT-10' LIMIT 1;
    SELECT "Id" INTO v_item_vlv_bal FROM "Master"."Item" WHERE "Code" = 'VLV-BAL-100' LIMIT 1;
    SELECT "Id" INTO v_item_gen_dsl FROM "Master"."Item" WHERE "Code" = 'GEN-DSL-500' LIMIT 1;
    SELECT "Id" INTO v_item_trf_oil FROM "Master"."Item" WHERE "Code" = 'TRF-OIL-1000' LIMIT 1;
    SELECT "Id" INTO v_item_cbl_cop FROM "Master"."Item" WHERE "Code" = 'CBL-COP-400' LIMIT 1;

    SELECT "Id" INTO v_unit_mt FROM "Master"."Unit" WHERE "Code" = 'MT' LIMIT 1;
    SELECT "Id" INTO v_unit_nos FROM "Master"."Unit" WHERE "Code" = 'NOS' LIMIT 1;
    SELECT "Id" INTO v_unit_mtr FROM "Master"."Unit" WHERE "Code" = 'MTR' LIMIT 1;
    SELECT "Id" INTO v_unit_set FROM "Master"."Unit" WHERE "Code" = 'SET' LIMIT 1;

    IF v_unit_mt IS NULL THEN v_unit_mt := 1; END IF;
    IF v_unit_nos IS NULL THEN v_unit_nos := 1; END IF;
    IF v_unit_mtr IS NULL THEN v_unit_mtr := 1; END IF;
    IF v_unit_set IS NULL THEN v_unit_set := 1; END IF;

    -- Clean up any previous seed auctions so this block can run idempotently
    DELETE FROM "AuctionRel"."AuctionStatement" WHERE "AuctionId" IN (SELECT "Id" FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%');
    DELETE FROM "AuctionRel"."BidDetail" WHERE "BidId" IN (SELECT "Id" FROM "AuctionRel"."Bid" WHERE "AuctionId" IN (SELECT "Id" FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%'));
    DELETE FROM "AuctionRel"."Bid" WHERE "AuctionId" IN (SELECT "Id" FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%');
    DELETE FROM "AuctionRel"."VendorIntent" WHERE "AuctionId" IN (SELECT "Id" FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%');
    DELETE FROM "AuctionRel"."AuctionRequirement" WHERE "AuctionId" IN (SELECT "Id" FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%');
    DELETE FROM "AuctionRel"."Auction" WHERE "DocNoYearly" LIKE 'AUC-2026-%';

    -- =========================================================================
    -- AUCTION 1: Tata Steel - Structural Steel Heavy Plates (LIVE / REVERSE)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_tata, v_org_tata, v_status_open, 'Mega Structural Steel Heavy Plates Procurement 2026', 'AUC-2026-TATA-001', CURRENT_DATE,
        NOW() - INTERVAL '3 hours', NOW() + INTERVAL '21 hours', false, false,
        true, 'National reverse auction for bulk supply of 2,500 MT high tensile structural steel plates conforming to IS 2062 Grade E350BR. Material must be supplied with original NABL accredited manufacturer test certificates.', NOW(), NOW()
    )
    RETURNING "Id" INTO v_auc1_id;

    IF v_item_stl_plt IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_tata, v_auc1_id, 1, v_item_stl_plt, '10mm Thickness, 2500mm Width x 12000mm Length, IS 2062 E350BR Normalized, Shot-blasted with primer coat', 2500, v_unit_mt
        )
        RETURNING "Id" INTO v_auc1_req1_id;

        -- Vendor Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc1_id, v_v_deltamach, true, true, v_t_deltamach),
               (v_auc1_id, v_v_titalloy, true, true, v_t_titalloy),
               (v_auc1_id, v_v_apexsteel, true, true, v_t_apexsteel);

        -- Initial Round of Bids (Replaced)
        -- Vendor Delta: Rate 58,000 -> Basic: 145,000,000, Tax: 26,100,000, Net: 171,100,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc1_id, v_v_deltamach, 145000000.00, 26100000.00, 0.00, 171100000.00, NOW() - INTERVAL '2 hours 45 minutes', false, 0, NULL)
        RETURNING "Id" INTO v_main_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_main_bid_id, v_auc1_req1_id, 58000.00, 145000000.00, 171100000.00);

        -- Vendor Titanium: Rate 56,500 -> Basic: 141,250,000, Tax: 25,425,000, Net: 166,675,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc1_id, v_v_titalloy, 141250000.00, 25425000.00, 0.00, 166675000.00, NOW() - INTERVAL '2 hours 20 minutes', false, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc1_req1_id, 56500.00, 141250000.00, 166675000.00);

        -- Active Competitive Round
        -- Delta Revised (Rank 3): Rate 54,200 -> Basic: 135,500,000, Tax: 24,390,000, Net: 159,890,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc1_id, v_v_deltamach, 135500000.00, 24390000.00, 0.00, 159890000.00, NOW() - INTERVAL '1 hour 15 minutes', true, 1, v_main_bid_id)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc1_req1_id, 54200.00, 135500000.00, 159890000.00);

        -- Titanium Revised (Rank 2): Rate 53,800 -> Basic: 134,500,000, Tax: 24,210,000, Net: 158,710,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc1_id, v_v_titalloy, 134500000.00, 24210000.00, 0.00, 158710000.00, NOW() - INTERVAL '40 minutes', true, 1, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc1_req1_id, 53800.00, 134500000.00, 158710000.00);

        -- Apex Steel (Rank 1 - L1 Leader!): Rate 52,900 -> Basic: 132,250,000, Tax: 23,805,000, Net: 156,055,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc1_id, v_v_apexsteel, 132250000.00, 23805000.00, 0.00, 156055000.00, NOW() - INTERVAL '12 minutes', true, 1, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc1_req1_id, 52900.00, 132250000.00, 156055000.00);
    END IF;

    -- =========================================================================
    -- AUCTION 2: Reliance Infra - Pipeline Trunnion Ball Valves (LIVE / REVERSE)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_rel, v_org_rel, v_status_open, 'Cross-Country Pipeline Trunnion Ball Valves Package', 'AUC-2026-REL-002', CURRENT_DATE,
        NOW() - INTERVAL '4 hours', NOW() + INTERVAL '20 hours', false, false,
        true, 'Procurement of Class 300 trunnion mounted flanged ball valves for cross-country hydrocarbon pipeline corridor. Valves must comply with API 6D and possess fire-safe testing certification according to API 607.', NOW(), NOW()
    )
    RETURNING "Id" INTO v_auc2_id;

    IF v_item_vlv_bal IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_rel, v_auc2_id, 1, v_item_vlv_bal, '4 Inch Class 300 Trunnion Ball Valve, ASTM A216 WCB Body, SS316 Ball & Stem, PTFE/Devlon Seats, Flanged RF ANSI B16.5', 60, v_unit_nos
        )
        RETURNING "Id" INTO v_auc2_req1_id;

        -- Vendor Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc2_id, v_v_pioneerfire, true, true, v_t_pioneerfire),
               (v_auc2_id, v_v_technopumps, true, true, v_t_technopumps),
               (v_auc2_id, v_v_apexvalves, true, true, v_t_apexvalves);

        -- Bids
        -- Pioneer Fire (Rank 3): Rate 91,000 -> Net: 6,442,800
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc2_id, v_v_pioneerfire, 5460000.00, 982800.00, 0.00, 6442800.00, NOW() - INTERVAL '3 hours 10 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc2_req1_id, 91000.00, 5460000.00, 6442800.00);

        -- TechnoPumps (Rank 2): Rate 86,500 -> Net: 6,124,200
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc2_id, v_v_technopumps, 5190000.00, 934200.00, 0.00, 6124200.00, NOW() - INTERVAL '2 hours', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc2_req1_id, 86500.00, 5190000.00, 6124200.00);

        -- Apex Valves (Rank 1 - L1 Leader!): Rate 82,000 -> Net: 5,805,600
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc2_id, v_v_apexvalves, 4920000.00, 885600.00, 0.00, 5805600.00, NOW() - INTERVAL '25 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc2_req1_id, 82000.00, 4920000.00, 5805600.00);
    END IF;

    -- =========================================================================
    -- AUCTION 3: L&T Heavy Engineering - Diesel Generator Sets (COMPLETED)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_lnt, v_org_lnt, v_status_completed, 'Auxiliary 500 kVA Diesel Generator Sets & Synchronization Panels', 'AUC-2026-LNT-003', CURRENT_DATE - 3,
        NOW() - INTERVAL '3 days', NOW() - INTERVAL '6 hours', false, false,
        true, 'Turnkey reverse auction for supply, factory testing, delivery, and commissioning of 500 kVA acoustic enclosed diesel generator sets with CPCB IV+ compliance for coastal fabrication yard substation.', NOW() - INTERVAL '4 days', NOW() - INTERVAL '6 hours'
    )
    RETURNING "Id" INTO v_auc3_id;

    IF v_item_gen_dsl IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_lnt, v_auc3_id, 1, v_item_gen_dsl, '500 kVA CPCB IV+ compliant diesel generator with electronic governor, digital synchronizing control panel, and residential silencer', 4, v_unit_set
        )
        RETURNING "Id" INTO v_auc3_req1_id;

        -- Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc3_id, v_v_crompton, true, true, v_t_crompton),
               (v_auc3_id, v_v_indogerman, true, true, v_t_indogerman),
               (v_auc3_id, v_v_bharattrf, true, true, v_t_bharattrf);

        -- Bids & Statements
        -- Crompton (Rank 3): Rate 4,300,000 -> Net: 20,296,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc3_id, v_v_crompton, 17200000.00, 3096000.00, 0.00, 20296000.00, NOW() - INTERVAL '2 days', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc3_req1_id, 4300000.00, 17200000.00, 20296000.00);
        INSERT INTO "AuctionRel"."AuctionStatement" ("AuctionId", "BidId", "VendorId", "Rank", "IsWinner", "TenantId")
        VALUES (v_auc3_id, v_bid_id, v_v_crompton, 3, false, v_t_lnt);

        -- IndoGerman (Rank 2): Rate 4,150,000 -> Net: 19,588,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc3_id, v_v_indogerman, 16600000.00, 2988000.00, 0.00, 19588000.00, NOW() - INTERVAL '1 day', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc3_req1_id, 4150000.00, 16600000.00, 19588000.00);
        INSERT INTO "AuctionRel"."AuctionStatement" ("AuctionId", "BidId", "VendorId", "Rank", "IsWinner", "TenantId")
        VALUES (v_auc3_id, v_bid_id, v_v_indogerman, 2, false, v_t_lnt);

        -- Bharat Electrical Transformers (Rank 1 - Winner!): Rate 4,000,000 -> Net: 18,880,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc3_id, v_v_bharattrf, 16000000.00, 2880000.00, 0.00, 18880000.00, NOW() - INTERVAL '8 hours', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc3_req1_id, 4000000.00, 16000000.00, 18880000.00);
        INSERT INTO "AuctionRel"."AuctionStatement" ("AuctionId", "BidId", "VendorId", "Rank", "IsWinner", "TenantId")
        VALUES (v_auc3_id, v_bid_id, v_v_bharattrf, 1, true, v_t_lnt);
    END IF;

    -- =========================================================================
    -- AUCTION 4: Adani Energy - Substation Oil Cooled Transformers (LIVE / REVERSE)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_adani, v_org_adani, v_status_open, 'High Voltage Substation 1000 kVA Oil Cooled Transformers', 'AUC-2026-ADANI-004', CURRENT_DATE,
        NOW() - INTERVAL '2 hours', NOW() + INTERVAL '46 hours', false, false,
        true, 'Bidding for 11kV/433V 1000 kVA copper-wound oil immersed distribution transformers with on-load tap changers (OLTC), nitrogen fire protection systems, and RTD temperature monitoring.', NOW(), NOW()
    )
    RETURNING "Id" INTO v_auc4_id;

    IF v_item_trf_oil IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_adani, v_auc4_id, 1, v_item_trf_oil, '1000 kVA 11kV/433V Dyn11, ONAN cooling, Copper wound, low-loss CRGO core, conforming to IS 1180 Level 2', 8, v_unit_nos
        )
        RETURNING "Id" INTO v_auc4_req1_id;

        -- Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc4_id, v_v_havells, true, true, v_t_havells),
               (v_auc4_id, v_v_abb, true, true, v_t_abb),
               (v_auc4_id, v_v_bharattrf, true, true, v_t_bharattrf);

        -- Bids
        -- Havells (Rank 3): Rate 2,310,000 -> Net: 21,806,400
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc4_id, v_v_havells, 18480000.00, 3326400.00, 0.00, 21806400.00, NOW() - INTERVAL '1 hour 40 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc4_req1_id, 2310000.00, 18480000.00, 21806400.00);

        -- ABB Heavy Power (Rank 2): Rate 2,240,000 -> Net: 21,145,600
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc4_id, v_v_abb, 17920000.00, 3225600.00, 0.00, 21145600.00, NOW() - INTERVAL '50 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc4_req1_id, 2240000.00, 17920000.00, 21145600.00);

        -- Bharat Electrical (Rank 1 - L1 Leader!): Rate 2,150,000 -> Net: 20,296,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc4_id, v_v_bharattrf, 17200000.00, 3096000.00, 0.00, 20296000.00, NOW() - INTERVAL '8 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc4_req1_id, 2150000.00, 17200000.00, 20296000.00);
    END IF;

    -- =========================================================================
    -- AUCTION 5: BHEL - High Tensile Boiler Plates (LIVE FORWARD AUCTION)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_bhel, v_org_bhel, v_status_open, 'Surplus High Tensile Boiler Plates & Structural Steel Scrap Lot', 'AUC-2026-BHEL-005', CURRENT_DATE,
        NOW() - INTERVAL '4 hours', NOW() + INTERVAL '30 hours', true, false,
        true, 'Forward auction (highest bid wins) for certified surplus prime boiler quality steel plates and heavy structural sections from power equipment manufacturing bay.', NOW(), NOW()
    )
    RETURNING "Id" INTO v_auc5_id;

    IF v_item_stl_plt IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_bhel, v_auc5_id, 1, v_item_stl_plt, 'Prime surplus ASTM A516 Gr 70 boiler quality steel plates, off-cuts and structural beams', 450, v_unit_mt
        )
        RETURNING "Id" INTO v_auc5_req1_id;

        -- Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc5_id, v_v_titalloy, true, true, v_t_titalloy),
               (v_auc5_id, v_v_deltamach, true, true, v_t_deltamach),
               (v_auc5_id, v_v_apexsteel, true, true, v_t_apexsteel);

        -- Bids (Forward Auction: Higher is better!)
        -- Titanium Alloys: Rate 40,000 -> Net: 21,240,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc5_id, v_v_titalloy, 18000000.00, 3240000.00, 0.00, 21240000.00, NOW() - INTERVAL '3 hours', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc5_req1_id, 40000.00, 18000000.00, 21240000.00);

        -- Delta Heavy: Rate 42,200 -> Net: 22,408,200
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc5_id, v_v_deltamach, 18990000.00, 3418200.00, 0.00, 22408200.00, NOW() - INTERVAL '1 hour 30 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc5_req1_id, 42200.00, 18990000.00, 22408200.00);

        -- Apex Steel (H1 Leader!): Rate 45,000 -> Net: 23,895,000
        INSERT INTO "AuctionRel"."Bid" ("AuctionId", "VendorId", "BasicAmount", "TaxAmount", "DiscountAmount", "NetAmount", "CreatedAt", "IsCurrent", "BidRevisionNo", "MainBidId")
        VALUES (v_auc5_id, v_v_apexsteel, 20250000.00, 3645000.00, 0.00, 23895000.00, NOW() - INTERVAL '15 minutes', true, 0, NULL)
        RETURNING "Id" INTO v_bid_id;
        INSERT INTO "AuctionRel"."BidDetail" ("BidId", "AuctionRequirementId", "Rate", "BaseAmount", "NetAmount")
        VALUES (v_bid_id, v_auc5_req1_id, 45000.00, 20250000.00, 23895000.00);
    END IF;

    -- =========================================================================
    -- AUCTION 6: Indian Oil - HT XLPE Armored Power Cables (SCHEDULED)
    -- =========================================================================
    INSERT INTO "AuctionRel"."Auction" (
        "TenantId", "OrganizationId", "StatusId", "AuctionName", "DocNoYearly", "DocDate",
        "AuctionStartTime", "AuctionEndTime", "IsForwardAuction", "IsBidPriceHidden",
        "OpenToAll", "About", "CreatedDateTime", "LastModifiedDateTime"
    )
    VALUES (
        v_t_iocl, v_org_iocl, v_status_scheduled, 'Refinery Grade HT XLPE Power Cables & Substation Instrumentation', 'AUC-2026-IOCL-006', CURRENT_DATE,
        NOW() + INTERVAL '18 hours', NOW() + INTERVAL '90 hours', false, false,
        true, 'Upcoming procurement tender for 3.5 Core 400 sq mm XLPE insulated aluminum/copper armored power cables conforming to IS 7098 (Part 2) for refinery modernization.', NOW(), NOW()
    )
    RETURNING "Id" INTO v_auc6_id;

    IF v_item_cbl_cop IS NOT NULL THEN
        INSERT INTO "AuctionRel"."AuctionRequirement" (
            "TenantId", "AuctionId", "LineNo", "ItemId", "TechnicalSpecification", "Quantity", "UnitId"
        ) VALUES (
            v_t_iocl, v_auc6_id, 1, v_item_cbl_cop, '3.5 Core 400 sq mm Stranded Compacted Circular Copper Conductor, Conductor Screen, XLPE Insulation, Armored, FRLS PVC Outer Sheath', 12000, v_unit_mtr
        )
        RETURNING "Id" INTO v_auc6_req1_id;

        -- Intents
        INSERT INTO "AuctionRel"."VendorIntent" ("AuctionId", "VendorId", "IsInterested", "IsQualified", "TenantId")
        VALUES (v_auc6_id, v_v_polycab, true, true, v_t_polycab),
               (v_auc6_id, v_v_havells, true, true, v_t_havells);
    END IF;

END $$;

