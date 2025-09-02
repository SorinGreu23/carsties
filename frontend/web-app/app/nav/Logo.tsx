'use client';
import { useParamsStore } from "@/hooks/useParamsStore";
import React from "react";
import { FaCar } from "react-icons/fa6";

export default function Logo() {
  const reset = useParamsStore(state => state.reset);

  return (
    <div onClick={reset} className="flex items-center gap-2 text-2xl font-semibold text-red-500">
      <FaCar size={25} />
      <div>Carsties Auctions</div>
    </div>
  );
}
