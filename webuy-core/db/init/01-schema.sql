-- Products (30,000+ SKUs)
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    sku VARCHAR(30) NOT NULL UNIQUE,
    barcode VARCHAR(14),
    name VARCHAR(200) NOT NULL,
    category VARCHAR(30) NOT NULL,
    sub_category VARCHAR(50),
    brand VARCHAR(100) NOT NULL,
    mrp NUMERIC(8,2) NOT NULL,
    selling_price NUMERIC(8,2) NOT NULL,
    discount_pct NUMERIC(4,1) NOT NULL DEFAULT 0,
    unit VARCHAR(20) NOT NULL,
    weight_grams INT,
    images TEXT[] NOT NULL DEFAULT '{}',
    description TEXT,
    expiry_date DATE,
    storage_temp NUMERIC(4,1),
    fssai_required BOOLEAN NOT NULL DEFAULT false,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_mrp CHECK (mrp > 0),
    CONSTRAINT chk_selling CHECK (selling_price > 0 AND selling_price <= mrp),
    CONSTRAINT chk_discount CHECK (discount_pct >= 0 AND discount_pct <= 90),
    CONSTRAINT chk_weight CHECK (weight_grams IS NULL OR (weight_grams >= 1 AND weight_grams <= 50000)),
    CONSTRAINT chk_storage_temp CHECK (storage_temp IS NULL OR (storage_temp >= -25 AND storage_temp <= 60)),
    CONSTRAINT chk_expiry CHECK (expiry_date IS NULL OR expiry_date > CURRENT_DATE),
    CONSTRAINT chk_category CHECK (category IN ('FRUITS','VEGETABLES','DAIRY','BAKERY','SNACKS',
        'BEVERAGES','ATTA_RICE','OILS_MASALA','PERSONAL_CARE','CLEANING','BABY_CARE','PET_FOOD',
        'ELECTRONICS','BEAUTY','PHARMACY','KITCHEN','PUJA','STATIONERY','PRINTOUTS',
        'HOME_OFFICE','ICE_CREAM','FROZEN'))
);

-- Dark Stores (every 2km)
CREATE TABLE dark_stores (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    lat NUMERIC(9,6) NOT NULL,
    lng NUMERIC(9,6) NOT NULL,
    address TEXT NOT NULL,
    pincode VARCHAR(6) NOT NULL,
    city VARCHAR(30) NOT NULL,
    delivery_radius_km NUMERIC(3,1) NOT NULL DEFAULT 2.0,
    open_time TIME NOT NULL DEFAULT '06:00',
    close_time TIME NOT NULL DEFAULT '23:00',
    cold_storage BOOLEAN NOT NULL DEFAULT false,
    fssai_license VARCHAR(14) NOT NULL,
    gstin VARCHAR(15) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    avg_pick_minutes INT NOT NULL DEFAULT 5,
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_lat CHECK (lat >= -90 AND lat <= 90),
    CONSTRAINT chk_lng CHECK (lng >= -180 AND lng <= 180),
    CONSTRAINT chk_radius CHECK (delivery_radius_km >= 0.5 AND delivery_radius_km <= 5.0),
    CONSTRAINT chk_pincode CHECK (pincode ~ '^[1-9][0-9]{5}$'),
    CONSTRAINT chk_fssai CHECK (fssai_license ~ '^[0-9]{14}$'),
    CONSTRAINT chk_gstin CHECK (gstin ~ '^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][0-9][A-Z0-9]Z[0-9A-Z]$'),
    CONSTRAINT chk_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_city CHECK (city IN ('AHMEDABAD','BENGALURU','CHANDIGARH','CHENNAI','DELHI',
        'FARIDABAD','GURGAON','HYDERABAD','JAIPUR','JALANDHAR','KANPUR','KOLKATA','LUCKNOW',
        'LUDHIANA','MEERUT','MOHALI','MUMBAI','PANCHKULA','PUNE','NOIDA','GHAZIABAD',
        'VADODARA','ZIRAKPUR'))
);

-- Inventory
CREATE TABLE inventory (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id),
    store_id UUID NOT NULL REFERENCES dark_stores(id),
    stock INT NOT NULL DEFAULT 0,
    reserved INT NOT NULL DEFAULT 0,
    shelf_location VARCHAR(20),
    expiry_batch DATE,
    last_restocked TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_stock CHECK (stock >= 0),
    CONSTRAINT chk_reserved CHECK (reserved >= 0 AND reserved <= stock),
    CONSTRAINT chk_available CHECK (stock - reserved >= 0),
    CONSTRAINT chk_cold_chain CHECK (
        (SELECT storage_temp FROM products WHERE id = product_id) IS NULL
        OR (SELECT storage_temp FROM products WHERE id = product_id) >= 5
        OR (SELECT cold_storage FROM dark_stores WHERE id = store_id) = true
    ),
    UNIQUE(product_id, store_id)
);

-- Consumers
CREATE TABLE consumers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    email VARCHAR(255),
    name VARCHAR(100) NOT NULL,
    wallet_balance NUMERIC(10,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_phone CHECK (phone ~ '^[0-9]{10,15}$'),
    CONSTRAINT chk_email CHECK (email IS NULL OR email ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT chk_wallet CHECK (wallet_balance >= 0),
    CONSTRAINT chk_status CHECK (status IN ('REGISTERED','VERIFIED','ACTIVE','SUSPENDED','DEACTIVATED'))
);

-- Delivery Partners
CREATE TABLE delivery_partners (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone VARCHAR(15) NOT NULL UNIQUE,
    name VARCHAR(100) NOT NULL,
    vehicle_type VARCHAR(20) NOT NULL,
    vehicle_reg VARCHAR(20) NOT NULL,
    assigned_store UUID REFERENCES dark_stores(id),
    current_lat NUMERIC(9,6),
    current_lng NUMERIC(9,6),
    rating NUMERIC(2,1) NOT NULL DEFAULT 5.0,
    earnings NUMERIC(12,2) NOT NULL DEFAULT 0,
    max_concurrent INT NOT NULL DEFAULT 3,
    kyc_status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    status VARCHAR(20) NOT NULL DEFAULT 'REGISTERED',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_vehicle_type CHECK (vehicle_type IN ('BICYCLE','BIKE','SCOOTER','CAR','AUTO')),
    CONSTRAINT chk_vehicle_reg CHECK (vehicle_reg ~ '^[A-Z]{2}\s[0-9]{2}\s[A-Z]{1,2}\s[0-9]{4}$'),
    CONSTRAINT chk_rating CHECK (rating >= 1.0 AND rating <= 5.0),
    CONSTRAINT chk_earnings CHECK (earnings >= 0),
    CONSTRAINT chk_max_concurrent CHECK (max_concurrent >= 1 AND max_concurrent <= 5),
    CONSTRAINT chk_kyc CHECK (kyc_status IN ('PENDING','VERIFIED','REJECTED')),
    CONSTRAINT chk_partner_status CHECK (status IN ('REGISTERED','KYC_PENDING','KYC_VERIFIED',
        'ONLINE','ASSIGNED','PICKING','DELIVERING','OFFLINE','SUSPENDED','DEACTIVATED'))
);

-- Orders
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ondc_txn_id UUID NOT NULL,
    ondc_message_id UUID NOT NULL,
    consumer_id UUID NOT NULL REFERENCES consumers(id),
    store_id UUID NOT NULL REFERENCES dark_stores(id),
    partner_id UUID REFERENCES delivery_partners(id),
    sub_total NUMERIC(8,2) NOT NULL,
    delivery_fee NUMERIC(6,2) NOT NULL DEFAULT 0,
    surge_fee NUMERIC(6,2) NOT NULL DEFAULT 0,
    discount NUMERIC(8,2) NOT NULL DEFAULT 0,
    total_fare NUMERIC(8,2) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    payment_status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    status VARCHAR(20) NOT NULL DEFAULT 'CREATED',
    otp VARCHAR(4) NOT NULL,
    delivery_address TEXT NOT NULL,
    delivery_lat NUMERIC(9,6) NOT NULL,
    delivery_lng NUMERIC(9,6) NOT NULL,
    delivery_eta_minutes INT NOT NULL,
    distance_km NUMERIC(5,2) NOT NULL,
    total_weight_grams INT,
    item_count INT NOT NULL,
    cancellation_reason VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    confirmed_at TIMESTAMPTZ,
    packed_at TIMESTAMPTZ,
    picked_up_at TIMESTAMPTZ,
    in_transit_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ,
    cancelled_at TIMESTAMPTZ,

    CONSTRAINT chk_sub_total CHECK (sub_total >= 99),
    CONSTRAINT chk_delivery_fee CHECK (delivery_fee >= 0),
    CONSTRAINT chk_surge_fee CHECK (surge_fee >= 0),
    CONSTRAINT chk_discount CHECK (discount >= 0),
    CONSTRAINT chk_total CHECK (total_fare >= 0 AND total_fare >= sub_total - discount),
    CONSTRAINT chk_otp CHECK (otp ~ '^[0-9]{4}$'),
    CONSTRAINT chk_eta CHECK (delivery_eta_minutes >= 5 AND delivery_eta_minutes <= 120),
    CONSTRAINT chk_distance CHECK (distance_km > 0 AND distance_km <= 10),
    CONSTRAINT chk_weight CHECK (total_weight_grams IS NULL OR total_weight_grams <= 15000),
    CONSTRAINT chk_item_count CHECK (item_count >= 1 AND item_count <= 50),
    CONSTRAINT chk_payment_method CHECK (payment_method IN ('UPI','CARD','WALLET','COD',
        'NET_BANKING','BNPL','SODEXO','PAYTM_FOOD','WEBUY_WALLET')),
    CONSTRAINT chk_payment_status CHECK (payment_status IN ('INITIATED','AUTHORIZED','CAPTURED',
        'SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_order_status CHECK (status IN ('CREATED','ACCEPTED','PACKED','PICKED_UP',
        'IN_TRANSIT','DELIVERED','CANCELLED','RETURNED','REFUNDED')),
    CONSTRAINT chk_confirmed_before_packed CHECK (packed_at IS NULL OR confirmed_at IS NOT NULL),
    CONSTRAINT chk_packed_before_picked CHECK (picked_up_at IS NULL OR packed_at IS NOT NULL),
    CONSTRAINT chk_picked_before_transit CHECK (in_transit_at IS NULL OR picked_up_at IS NOT NULL),
    CONSTRAINT chk_transit_before_delivered CHECK (delivered_at IS NULL OR in_transit_at IS NOT NULL),
    CONSTRAINT chk_cancelled_no_delivery CHECK (status != 'CANCELLED' OR delivered_at IS NULL),
    CONSTRAINT chk_delivered_no_cancel CHECK (status != 'DELIVERED' OR cancelled_at IS NULL),
    CONSTRAINT chk_store_distance CHECK (distance_km <= (
        SELECT delivery_radius_km FROM dark_stores WHERE id = store_id
    ))
);

-- Order Items
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    product_id UUID NOT NULL REFERENCES products(id),
    sku VARCHAR(30) NOT NULL,
    name VARCHAR(200) NOT NULL,
    quantity INT NOT NULL,
    unit_price NUMERIC(8,2) NOT NULL,
    total_price NUMERIC(8,2) NOT NULL,
    weight_grams INT,
    storage_temp NUMERIC(4,1),
    substitution_policy VARCHAR(20) NOT NULL DEFAULT 'NO_SUBSTITUTE',

    CONSTRAINT chk_quantity CHECK (quantity >= 1 AND quantity <= 99),
    CONSTRAINT chk_unit_price CHECK (unit_price > 0),
    CONSTRAINT chk_total_price CHECK (total_price = unit_price * quantity),
    CONSTRAINT chk_substitution CHECK (substitution_policy IN ('NO_SUBSTITUTE','ALLOW_SIMILAR','ALLOW_ANY','CALL_ME'))
);

-- Payments
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    method VARCHAR(20) NOT NULL,
    amount NUMERIC(8,2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'INITIATED',
    gateway_ref VARCHAR(100),
    upi_txn_id VARCHAR(50),
    initiated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    authorized_at TIMESTAMPTZ,
    captured_at TIMESTAMPTZ,
    settled_at TIMESTAMPTZ,
    refunded_at TIMESTAMPTZ,

    CONSTRAINT chk_amount CHECK (amount > 0),
    CONSTRAINT chk_payment_status CHECK (status IN ('INITIATED','AUTHORIZED','CAPTURED',
        'SETTLED','FAILED','REFUND_INITIATED','REFUNDED')),
    CONSTRAINT chk_auth_before_capture CHECK (captured_at IS NULL OR authorized_at IS NOT NULL),
    CONSTRAINT chk_capture_before_settle CHECK (settled_at IS NULL OR captured_at IS NOT NULL)
);

-- Subscriptions
CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    consumer_id UUID NOT NULL REFERENCES consumers(id),
    frequency VARCHAR(20) NOT NULL,
    delivery_start TIME NOT NULL DEFAULT '06:00',
    delivery_end TIME NOT NULL DEFAULT '08:00',
    next_delivery TIMESTAMPTZ NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'ACTIVE',
    payment_method VARCHAR(20) NOT NULL,
    pause_until TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_frequency CHECK (frequency IN ('DAILY','ALTERNATE_DAYS','WEEKLY','BIWEEKLY','MONTHLY')),
    CONSTRAINT chk_delivery_window CHECK (delivery_start >= '06:00' AND delivery_end <= '22:00'),
    CONSTRAINT chk_sub_status CHECK (status IN ('ACTIVE','PAUSED','SKIPPED','CANCELLED','EXPIRED'))
);

-- Ratings
CREATE TABLE ratings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL UNIQUE REFERENCES orders(id),
    rater_type VARCHAR(10) NOT NULL,
    rater_id UUID NOT NULL,
    rated_id UUID NOT NULL,
    score NUMERIC(2,1) NOT NULL,
    comment VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_rater_type CHECK (rater_type IN ('CONSUMER','PARTNER')),
    CONSTRAINT chk_score CHECK (score >= 1.0 AND score <= 5.0),
    CONSTRAINT chk_rating_window CHECK (created_at <= (
        SELECT delivered_at FROM orders WHERE id = order_id
    ) + INTERVAL '48 hours')
);

-- ONDC Transaction Log
CREATE TABLE ondc_transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    txn_id UUID NOT NULL,
    message_id UUID NOT NULL,
    api VARCHAR(20) NOT NULL,
    direction VARCHAR(10) NOT NULL,
    buyer_subscriber_id VARCHAR(50) NOT NULL,
    seller_subscriber_id VARCHAR(50) NOT NULL,
    payload JSONB NOT NULL,
    signature TEXT NOT NULL,
    ack_status VARCHAR(10) NOT NULL DEFAULT 'PENDING',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT chk_api CHECK (api IN ('SEARCH','SELECT','INIT','CONFIRM','STATUS',
        'TRACK','CANCEL','UPDATE','RATING','SUPPORT')),
    CONSTRAINT chk_direction CHECK (direction IN ('REQUEST','RESPONSE')),
    CONSTRAINT chk_ack CHECK (ack_status IN ('PENDING','ACK','NACK')),
    CONSTRAINT chk_ack_deadline CHECK (
        ack_status != 'PENDING' OR created_at > now() - INTERVAL '30 seconds'
    )
);
