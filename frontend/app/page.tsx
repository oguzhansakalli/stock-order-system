import DashboardLayout from '@/components/layout/dashboard-layout';

export default function HomePage() {
  return (
    <DashboardLayout>
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-4">
          Welcome to Stock Order System
        </h1>
        <p className="text-gray-600">
          Dashboard statistics and charts will be added here...
        </p>
        
        {/* Placeholder cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mt-6">
          {[
            { title: 'Total Orders', value: '0', color: 'bg-blue-500' },
            { title: 'Total Revenue', value: '$0', color: 'bg-green-500' },
            { title: 'Products', value: '0', color: 'bg-purple-500' },
            { title: 'Active Orders', value: '0', color: 'bg-orange-500' },
          ].map((stat) => (
            <div
              key={stat.title}
              className="bg-white rounded-lg shadow p-6"
            >
              <div className="flex items-center">
                <div className={`${stat.color} w-12 h-12 rounded-lg flex items-center justify-center text-white text-xl font-bold`}>
                  {stat.value.charAt(0)}
                </div>
                <div className="ml-4">
                  <p className="text-gray-500 text-sm">{stat.title}</p>
                  <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </DashboardLayout>
  );
}