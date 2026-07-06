"use client";

import { useState } from "react";

export default function WalletDashboard() {
  const [walletId, setWalletId] = useState<string | null>(null);
  const [balance, setBalance] = useState<number>(0);
  const [amount, setAmount] = useState<string>("");
  const [status, setStatus] = useState<string>("");
  const [loading, setLoading] = useState(false);

  const createWallet = async () => {
    setLoading(true);
    try {
      const res = await fetch("http://localhost:5000/api/wallets", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          customerId: "11111111-1111-1111-1111-111111111111",
          currency: "USD",
        }),
      });
      const data = await res.json();
      setWalletId(data.walletId);
      setStatus("Wallet created successfully.");
    } catch (err: any) {
      setStatus("Error: " + err.message);
    }
    setLoading(false);
  };

  const getBalance = async () => {
    if (!walletId) return;
    try {
      const res = await fetch(`http://localhost:5000/api/wallets/${walletId}/balance`);
      if (res.ok) {
        const data = await res.json();
        setBalance(data.availableBalance);
      }
    } catch (err: any) {
      setStatus("Error: " + err.message);
    }
  };

  const handleTransaction = async (type: "credit" | "debit") => {
    if (!walletId) return;
    setLoading(true);
    try {
      const val = parseFloat(amount);
      if (isNaN(val) || val <= 0) {
        setStatus("Amount must be greater than 0");
        setLoading(false);
        return;
      }
      
      const res = await fetch(`http://localhost:5000/api/wallets/${walletId}/${type}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          amount: val,
          referenceId: crypto.randomUUID(),
        }),
      });
      
      if (res.ok) {
        setStatus(`${type.toUpperCase()} successful.`);
        await getBalance();
        setAmount("");
      } else {
        const errText = await res.text();
        setStatus(`Error: ${res.status} - ${errText}`);
      }
    } catch (err: any) {
      setStatus("Error: " + err.message);
    }
    setLoading(false);
  };

  return (
    <div style={{ padding: "40px", fontFamily: "system-ui", maxWidth: "600px", margin: "0 auto", backgroundColor: "#f9fafb", borderRadius: "8px", boxShadow: "0 4px 6px rgba(0,0,0,0.1)", marginTop: "40px" }}>
      <h1 style={{ color: "#111827", fontSize: "24px", fontWeight: "bold", marginBottom: "20px" }}>Fintech Wallet Dashboard</h1>
      
      {!walletId ? (
        <button 
          onClick={createWallet} 
          disabled={loading}
          style={{ padding: "10px 20px", backgroundColor: "#4f46e5", color: "white", border: "none", borderRadius: "4px", cursor: "pointer", fontWeight: "500" }}
        >
          {loading ? "Creating..." : "Create New Wallet"}
        </button>
      ) : (
        <div>
          <div style={{ backgroundColor: "white", padding: "20px", borderRadius: "6px", marginBottom: "20px", border: "1px solid #e5e7eb" }}>
            <p style={{ margin: "0 0 10px 0", color: "#6b7280", fontSize: "14px" }}>Wallet ID: {walletId}</p>
            <h2 style={{ margin: "0", fontSize: "36px", color: "#111827" }}>${balance.toFixed(2)}</h2>
            <p style={{ margin: "5px 0 0 0", color: "#10b981", fontSize: "14px", fontWeight: "500" }}>Available Balance</p>
          </div>

          <div style={{ display: "flex", gap: "10px", marginBottom: "20px" }}>
            <input 
              type="number" 
              value={amount} 
              onChange={e => setAmount(e.target.value)} 
              placeholder="Amount (USD)"
              style={{ flex: 1, padding: "10px", border: "1px solid #d1d5db", borderRadius: "4px", fontSize: "16px" }}
            />
            <button 
              onClick={() => handleTransaction("credit")} 
              disabled={loading}
              style={{ padding: "10px 20px", backgroundColor: "#10b981", color: "white", border: "none", borderRadius: "4px", cursor: "pointer", fontWeight: "500" }}
            >
              Deposit
            </button>
            <button 
              onClick={() => handleTransaction("debit")} 
              disabled={loading}
              style={{ padding: "10px 20px", backgroundColor: "#ef4444", color: "white", border: "none", borderRadius: "4px", cursor: "pointer", fontWeight: "500" }}
            >
              Withdraw
            </button>
          </div>
        </div>
      )}
      
      {status && (
        <div style={{ marginTop: "20px", padding: "10px", backgroundColor: "#f3f4f6", borderRadius: "4px", color: "#374151", fontSize: "14px" }}>
          {status}
        </div>
      )}
    </div>
  );
}
