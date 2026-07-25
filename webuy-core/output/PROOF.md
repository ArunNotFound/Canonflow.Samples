# CanonFlow DB-to-F# Constraint Fidelity Proof

This document validates that all PostgreSQL DB constraints mapped to an exact F# Smart Constructor.

| DB Constraint | F# Smart Constructor | Status |
|---------------|----------------------|--------|
| `products.chk_mrp` (mrp > 0) | `Money` | **Exact** |
| `products.chk_discount` (0 to 90) | `DiscountPct` | **Exact** |
| `products.chk_weight` (1 to 50000) | `WeightGrams` | **Exact** |
| `products.chk_expiry` (> CURRENT_DATE) | `ExpiryDate` | **Exact** |
| `dark_stores.chk_lat` (-90 to 90) | `GeoCoord.lat` | **Exact** |
| `dark_stores.chk_pincode` (6 digits, no lead 0)| `Pincode` | **Exact** |
| `dark_stores.chk_gstin` (Regex) | `GSTIN` | **Exact** |
| `inventory.chk_available` (stock - reserved) | `InventoryItem.validate`| **Exact** |
| `consumers.chk_phone` (10-15 digits) | `PhoneNumber` | **Exact** |
| `delivery_partners.chk_rating` (1.0 - 5.0) | `Rating` | **Exact** |
| `orders.chk_total` (fare >= sub_total - disc) | `FareCalculation` | **Exact** |
| `orders.chk_otp` (4 digits) | `OTP` | **Exact** |
| `orders.chk_store_distance` (radius check) | `validateStoreDistance`| **Exact** |

**Zero "Unsupported" mapping rows found. CanonFlow fidelity is 100%.**
