import * as signalR from "@microsoft/signalr";

const HUB_URL =
  process.env.NEXT_PUBLIC_SIGNALR_HUB_URL ||
  "https://localhost:5001/hubs/orders";

export class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private isConnecting = false;

  async connect(token: string): Promise<void> {
    if (this.connection || this.isConnecting) {
      console.log("Already connected or connecting");
      return;
    }

    this.isConnecting = true;

    try {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL, {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Connection events
      this.connection.onclose(() => {
        console.log("SignalR disconnected");
      });

      this.connection.onreconnecting(() => {
        console.log("SignalR reconnecting...");
      });

      this.connection.onreconnected(() => {
        console.log("SignalR reconnected");
      });

      await this.connection.start();
      console.log("SignalR connected successfully");
    } catch (error) {
      console.error("SignalR connection error:", error);
    } finally {
      this.isConnecting = false;
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  // Subscribe to OrderCreated event
  onOrderCreated(callback: (data: any) => void): void {
    if (!this.connection) {
      console.warn("SignalR not connected");
      return;
    }
    this.connection.on("OrderCreated", callback);
  }

  // Subscribe to OrderStatusChanged event
  onOrderStatusChanged(callback: (data: any) => void): void {
    if (!this.connection) {
      console.warn("SignalR not connected");
      return;
    }
    this.connection.on("OrderStatusChanged", callback);
  }

  // Subscribe to OrderCancelled event
  onOrderCancelled(callback: (data: any) => void): void {
    if (!this.connection) {
      console.warn("SignalR not connected");
      return;
    }
    this.connection.on("OrderCancelled", callback);
  }

  // Unsubscribe from events
  off(eventName: string): void {
    if (this.connection) {
      this.connection.off(eventName);
    }
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

// Singleton instance
export const signalRService = new SignalRService();
