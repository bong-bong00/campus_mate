import type { Metadata } from "next";
import AppHeader from "@/components/AppHeader";
import "./globals.css";

export const metadata: Metadata = {
  title: "Campus Signal",
  description: "공모전 정보를 모아 내 프로필과 대조하고, 지금 해야 할 일로 알려줍니다",
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="ko">
      <body className="min-h-screen bg-surface text-body antialiased">
        <AppHeader />
        <main>{children}</main>
      </body>
    </html>
  );
}
