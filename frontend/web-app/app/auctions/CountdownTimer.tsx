'use client';
import Countdown, { zeroPad } from "react-countdown";

const renderer = ({
  days,
  hours,
  minutes,
  seconds,
  completed,
}: {
  days: number;
  hours: number;
  minutes: number;
  seconds: number;
  completed: boolean;
}) => {
  return (
    <div
      className={`
      border-2 border-white text-white py-1 px-2 rounded-lg flex justify-center
      ${
        completed
          ? "bg-red-600"
          : days === 0 && hours < 10
          ? "bg-amber-600"
          : "bg-green-600"
      }
    `}
    >
      {completed ? (
        <span>Auction finished</span>
      ) : (
        <span suppressHydrationWarning={true}>
          {days}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
        </span>
      )}
    </div>
  );
};

type Props = {
  auctionEnd: string;
}

export default function CountdownTimer({auctionEnd}: Props) {
  if (!auctionEnd) {
    return <span className="text-gray-500">No end date</span>;
  }

  // Convert to Date object to ensure proper parsing
  const endDate = new Date(auctionEnd);
  
  // Check if the date is valid
  if (isNaN(endDate.getTime())) {
    return <span className="text-gray-500">Invalid date</span>;
  }

  return (
    <div className="text-sm">
      <Countdown 
        date={endDate} 
        renderer={renderer}
      />
    </div>
  );
}
