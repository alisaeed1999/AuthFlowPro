// src/app/core/utils/jwt-utils.ts
import { jwtDecode } from "jwt-decode";

export function isTokenExpiringSoon(token: string, thresholdSeconds = 60): boolean {
  if (!token) return true;
  try {
    const decoded: any = jwtDecode(token);
    
    const exp = decoded.exp; // seconds since epoch
    const now = Math.floor(Date.now() / 1000);

    return exp - now < thresholdSeconds; // true if about to expire
  } catch (err) {
    return true; // if it can't decode, treat as expired
  }
}
