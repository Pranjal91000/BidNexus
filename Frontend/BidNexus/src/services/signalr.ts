import { useEffect, useRef, useState, useCallback } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { API_BASE_URL } from './api';
import type { SignalRStatus } from '../types';

export interface BidAcceptedPayload {
  auctionId?: number;
  AuctionId?: number;
  bidId?: number;
  BidId?: number;
  vendorId?: number;
  VendorId?: number;
  netAmount?: number;
  NetAmount?: number;
}

export interface AuctionClosedPayload {
  auctionId?: number;
  AuctionId?: number;
}

export function useAuctionSignalR(
  token: string,
  auctionId: number | null,
  onBidAccepted?: (payload: BidAcceptedPayload) => void,
  onAuctionClosed?: (payload: AuctionClosedPayload) => void
) {
  const [status, setStatus] = useState<SignalRStatus>('DISCONNECTED');
  const connectionRef = useRef<HubConnection | null>(null);

  const connect = useCallback(async () => {
    if (!token || !auctionId) return;

    if (connectionRef.current) {
      try {
        await connectionRef.current.stop();
      } catch (err) {
        console.warn('Error stopping previous SignalR connection', err);
      }
    }

    const hubUrl = `${API_BASE_URL}/hubs/auction`;
    
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount >= 10) return null;
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 10000);
        }
      })
      .configureLogging(LogLevel.Warning)
      .build();

    connection.onreconnecting(() => {
      setStatus('RECONNECTING');
    });

    connection.onreconnected(() => {
      setStatus('CONNECTED');
      if (auctionId) {
        connection.invoke('JoinAuction', auctionId).catch((e) => {
          console.warn('Failed to rejoin auction room', e);
        });
      }
    });

    connection.onclose(() => {
      setStatus('DISCONNECTED');
    });

    connection.on('BidAccepted', (data: BidAcceptedPayload) => {
      const payloadAuctionId = data.AuctionId ?? data.auctionId;
      if (!auctionId || payloadAuctionId === auctionId) {
        onBidAccepted?.(data);
      }
    });

    connection.on('AuctionClosed', (data: AuctionClosedPayload) => {
      const payloadAuctionId = data.AuctionId ?? data.auctionId;
      if (!auctionId || payloadAuctionId === auctionId) {
        onAuctionClosed?.(data);
      }
    });

    setStatus('CONNECTING');

    try {
      await connection.start();
      setStatus('CONNECTED');
      await connection.invoke('JoinAuction', auctionId);
      connectionRef.current = connection;
    } catch (err) {
      console.warn('SignalR Connection Failed:', err);
      setStatus('DISCONNECTED');
    }
  }, [token, auctionId, onBidAccepted, onAuctionClosed]);

  useEffect(() => {
    if (token && auctionId) {
      connect();
    }

    return () => {
      if (connectionRef.current) {
        const conn = connectionRef.current;
        connectionRef.current = null;
        if (auctionId && conn.state === HubConnectionState.Connected) {
          conn.invoke('LeaveAuction', auctionId).catch(() => {});
        }
        conn.stop().catch(() => {});
      }
    };
  }, [token, auctionId, connect]);

  return {
    status,
    reconnect: connect
  };
}
