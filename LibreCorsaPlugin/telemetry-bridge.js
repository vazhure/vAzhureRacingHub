#!/usr/bin/env node
/**
 * LibreCorsa Telemetry-Only Bridge
 * =================================
 * 
 * Минимальный bridge БЕЗ FFB — только телеметрия.
 * Оригинальный npm run ffb завершается, если не найден FFB-руль.
 * Этот скрипт оставляет WebSocket+UDP работающими без руля.
 * 
 * Запуск:
 *   1. Скопируй этот файл в папку librecorsa (где уже npm install)
 *   2. node telemetry-bridge.js
 *   3. Открой LibreCorsa в браузере
 * 
 * Или standalone:
 *   npm install ws
 *   node telemetry-bridge.js
 * 
 * Если игра на librecorsa.org (не локально):
 *   PowerShell: $env:FFB_ALLOW_ORIGIN="https://librecorsa.org"; node telemetry-bridge.js
 *   cmd:         set "FFB_ALLOW_ORIGIN=https://librecorsa.org" && node telemetry-bridge.js
 */

const http = require('http');
const dgram = require('dgram');
const path = require('path');

// Загружаем ws из node_modules librecorsa или из текущей папки
let WebSocketServer;
try {
    WebSocketServer = require('ws').WebSocketServer;
} catch (e) {
    console.error('ERROR: package "ws" not found.');
    console.error('Run this script from the librecorsa folder (after npm install),');
    console.error('or install ws separately: npm install ws');
    process.exit(1);
}

const PORT = parseInt(process.env.FFB_PORT || '18500', 10);
const ALLOW_ORIGIN = process.env.FFB_ALLOW_ORIGIN || null;
const udpSocket = dgram.createSocket('udp4');
let packetCount = 0;
let connectedClient = null;

// HTTP статус-сервер (браузер проверяет, что bridge жив)
const server = http.createServer((req, res) => {
    res.writeHead(200, {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
    });
    res.end(JSON.stringify({
        bridge: 'librecorsa-telemetry-only',
        port: PORT,
        status: 'running',
        ffb: false,
        telemetry: true,
        packets: packetCount,
    }));
});

// WebSocket сервер — браузер подключается сюда
const wss = new WebSocketServer({ server });

function originAllowed(origin) {
    if (!origin) return true;                          // без origin (localhost)
    if (origin.startsWith('http://localhost')) return true;
    if (origin.startsWith('http://127.0.0.1')) return true;
    if (ALLOW_ORIGIN && origin === ALLOW_ORIGIN) return true;
    return false;
}

wss.on('connection', (ws, req) => {
    const origin = req.headers.origin || 'none';
    if (!originAllowed(origin)) {
        console.log(`[!] Connection rejected: origin "${origin}" not allowed`);
        console.log('    Set FFB_ALLOW_ORIGIN env var (see instructions in script header)');
        ws.close();
        return;
    }

    // Если уже есть подключённый клиент — заменяем
    if (connectedClient && connectedClient !== ws) {
        try { connectedClient.close(); } catch {}
    }
    connectedClient = ws;

    console.log(`[${new Date().toLocaleTimeString()}] ✓ Browser connected (origin: ${origin})`);

    // Отправляем статус браузеру — сообщаем, что bridge готов,
    // FFB отключён, телеметрия включена
    ws.send(JSON.stringify({
        type: 'bridge-status',
        ffb: false,
        telemetry: true,
        bridge: 'librecorsa-telemetry-only',
    }));

    ws.on('message', (raw) => {
        let msg;
        try {
            msg = JSON.parse(raw.toString());
        } catch {
            return; // не JSON — игнорируем
        }

        if (msg.t === 'tel' && msg.port && msg.d) {
            // ТЕЛЕМЕТРИЯ — пересылаем как UDP JSON
            const json = JSON.stringify(msg.d);
            const buf = Buffer.from(json, 'utf-8');
            udpSocket.send(buf, 0, buf.length, msg.port, '127.0.0.1', (err) => {
                if (err) console.error('  UDP send error:', err.message);
            });
            packetCount++;

            // Печатаем краткую сводку каждые 60 пакетов (~1 сек)
            if (packetCount % 60 === 0 || packetCount === 1) {
                const d = msg.d;
                const speed = (d.speedKmh || 0).toFixed(1);
                const rpm = (d.rpm || 0).toFixed(0);
                const gear = d.gear || 0;
                const car = d.car || '?';
                const track = d.track || '?';
                console.log(
                    `  [${new Date().toLocaleTimeString()}] #${packetCount} | ` +
                    `${car} ${track} | ${speed} km/h ${rpm} RPM G${gear} → :${msg.port}`
                );
            }
        }
        // FFB-команды (msg.t === 'ffb' и т.п.) — игнорируем, нет руля
    });

    ws.on('close', () => {
        console.log(`[${new Date().toLocaleTimeString()}] ✗ Browser disconnected`);
        if (connectedClient === ws) connectedClient = null;
    });

    ws.on('error', () => {
        if (connectedClient === ws) connectedClient = null;
    });
});

// Старт
server.listen(PORT, '127.0.0.1', () => {
    console.log('');
    console.log('╔══════════════════════════════════════════════════╗');
    console.log('║   LibreCorsa Telemetry-Only Bridge               ║');
    console.log('║   (no FFB wheel required)                         ║');
    console.log('╚══════════════════════════════════════════════════╝');
    console.log('');
    console.log(`  WebSocket :  ws://127.0.0.1:${PORT}`);
    console.log(`  Telemetry :  UDP → 127.0.0.1:<port from game>`);
    console.log(`  FFB       :  disabled (no wheel needed)`);
    if (ALLOW_ORIGIN) {
        console.log(`  Origin    :  ${ALLOW_ORIGIN}`);
    } else {
        console.log(`  Origin    :  localhost only`);
        console.log(`               (set FFB_ALLOW_ORIGIN for librecorsa.org)`);
    }
    console.log('');
    console.log('  Waiting for browser connection...');
    console.log('  Open LibreCorsa in your browser now.');
    console.log('');
    console.log('  Ctrl+C to stop');
    console.log('');
});

process.on('SIGINT', () => {
    console.log('\nStopping bridge...');
    try { wss.close(); } catch {}
    try { server.close(); } catch {}
    try { udpSocket.close(); } catch {}
    process.exit(0);
});
