import withFlowbiteReact from "flowbite-react/plugin/nextjs";

/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {protocol: 'https', hostname: 'cdn.pixabay.com'}
    ]
  }
};

export default withFlowbiteReact(nextConfig);