import React from 'react'
import { FaCar } from 'react-icons/fa6'

export default function Navbar() {
    return (
        <header className='sticky top-0 z-50 flex justify-between bg-white p-5 item-center text-gray-800 shadow-md'>
            <div className='flex items-center gap-2 text-2xl font-semibold text-red-500'>
              <FaCar size={25}/>
              <div>Carsties Auctions</div>
            </div>
            <div>Middle</div>
            <div>Right</div>
        </header>
    )
}   
