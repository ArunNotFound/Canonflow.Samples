import { z } from "zod";

// C2: F# Smart Constructors translated to Zod

export const PhoneNumberSchema = z.string().regex(/^[0-9]{10,15}$/, "Invalid phone number length or format");
export const EmailSchema = z.string().email("Invalid email format");
export const PincodeSchema = z.string().regex(/^[1-9][0-9]{5}$/, "Invalid pincode");
export const GSTINSchema = z.string().regex(/^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][0-9][A-Z0-9]Z[0-9A-Z]$/, "Invalid GSTIN");
export const FSSAISchema = z.string().regex(/^[0-9]{14}$/, "Invalid FSSAI License");
export const GeoCoordSchema = z.object({
  lat: z.number().min(-90).max(90),
  lng: z.number().min(-180).max(180)
});
export const DistanceSchema = z.number().positive().max(10);
export const DeliveryRadiusSchema = z.number().min(0.5).max(5.0);
export const DeliveryETASchema = z.number().int().min(5).max(120);
export const MoneySchema = z.number().nonnegative().multipleOf(0.01);
export const QuantitySchema = z.number().int().min(1).max(99);
export const WeightGramsSchema = z.number().int().min(1).max(50000);
export const DiscountPctSchema = z.number().min(0).max(90);
export const SurgeMultiplierSchema = z.number().min(1.0).max(3.0);
export const SKUSchema = z.string().regex(/^[A-Z0-9-]+$/).min(6).max(30);
export const BarcodeSchema = z.string().regex(/^[0-9]{8,14}$/);
export const ExpiryDateSchema = z.date().refine((d) => d > new Date(), { message: "Expiry date must be in the future" });
export const TemperatureCelsiusSchema = z.number().min(-25).max(60);
export const RatingSchema = z.number().min(1.0).max(5.0).multipleOf(0.1);
export const OTPSchema = z.string().regex(/^[0-9]{4}$/);
export const SubscriberIdSchema = z.string().regex(/^[a-z0-9.]+$/);

export const OrderStatusEnum = z.enum([
  "Created", "Accepted", "Packed", "PickedUp", "InTransit", "Delivered", "Cancelled", "Returned", "Refunded"
]);

// ... other enums mapping 1:1 with Domain.Enums ...
