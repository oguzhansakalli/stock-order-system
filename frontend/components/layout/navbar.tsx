"use client";

import { useAuth } from "@/lib/auth/auth-context";
import { BellIcon, UserCircleIcon } from "@heroicons/react/24/outline";

export default function Navbar() {
  const { user, logout } = useAuth();
  return (
    <nav className="bg-white shadow-sm border-b border-gray-200">
      <div className="px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          {/* Left side - could add breadcrumbs here later */}
          <div className="flex items-center">
            <h2 className="text-xl font-semibold text-gray-800">Dashboard</h2>
          </div>

          {/* Right side - user menu */}
          <div className="flex items-center space-x-4">
            {/* Notifications (placeholder) */}
            <button className="p-2 text-gray-400 hover:text-gray-600 relative">
              <BellIcon className="w-6 h-6" />
              <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
            </button>

            {/* User menu */}
            <div className="flex items-center space-x-3">
              <div className="text-right">
                <p className="text-sm font-medium text-gray-700">
                  {user?.name}
                </p>
                <p className="text-xs text-gray-500">{user?.role}</p>
              </div>

              <div className="relative group">
                <button className="flex items-center">
                  <UserCircleIcon className="w-8 h-8 text-gray-400" />
                </button>

                {/* Dropdown */}
                <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 hidden group-hover:block z-10">
                  <button
                    onClick={logout}
                    className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  >
                    Sign out
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}
