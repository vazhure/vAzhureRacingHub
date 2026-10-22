#!/usr/bin/env python3
"""Тест приёмника UDP-телеметрии от motion_udp.lua (openOMSI)."""
import json
import socket
import os

PORT = 4123

# Инициализация сокета
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(("127.0.0.1", PORT))

# Включаем поддержку ANSI-кодов в Windows-терминале (если это необходимо)
os.system('')

# Полностью очищаем экран один раз при старте
print("\033[2J\033[H", end="")
print(f"Слушаю UDP 127.0.0.1:{PORT} ...")
print("-" * 80)

while True:
    data, _ = sock.recvfrom(8192)
    try:
        t = json.loads(data.decode("utf-8"))
    except (UnicodeDecodeError, json.JSONDecodeError):
        continue

    # Возвращаем курсор в начало (после первой строчки с портом)
    # \033[3;1H означает: 3-я строка, 1-я колонка терминала
    print("\033[3;1H", end="")

    # Передача / КПП
    gear = "N"
    if t['cockpit_gangR'] > 0: gear = "R"
    elif t['cockpit_gang1'] > 0: gear = "1"
    elif t['cockpit_gang2'] > 0: gear = "2"
    elif t['cockpit_gang3'] > 0: gear = "3"
    elif t['cockpit_gangN'] > 0: gear = "N"
    
    # Поворотники
    blinkers = "ВЫКЛ"
    if t['AI_Blinker_L'] > 0 and t['AI_Blinker_R'] > 0: blinkers = "АВАРИЙКА"
    elif t['AI_Blinker_L'] > 0: blinkers = "ЛЕВЫЙ"
    elif t['AI_Blinker_R'] > 0: blinkers = "ПРАВЫЙ"

    # Вывод телеметрии с фиксированной шириной строк
    print(f" СТАТУС:     Пауза: {int(t['pause'])}  | Скорость: {t['velocity']:5.1f} м/с                       ")
    print(f" ПЕДАЛИ:     Газ: {t['throttle']:4.2f} | Тормоз: {t['brake']:4.2f} | Сцепление: {t['clutch']:4.2f} | Сила торм.: {t['brakeforce']:4.2f}   ")
    print(f" ДВИЖЕНИЕ:   Surge (Продольное): {t['surge']:+.2f} | Sway (Боковое): {t['sway']:+.2f} | Heave (Верт.): {t['heave']:+.2f}   ")
    print(f" УГЛЫ КУЗОВА: Roll (Крен): {t['roll']:+.1f}° | Pitch (Тангаж): {t['pitch']:+.1f}° | Yaw (Рыскание): {t['yaw']:+.1f}°   ")
    print(f" СЫРЫЕ УСК.: Acc X: {t['acc_x']:+.2f} | Acc Y: {t['acc_y']:+.2f} | Acc Z: {t['acc_z']:+.2f}   ")
    print(f" ПОДВЕСКА:   Ось0 Л: {t['susp_0_l']:.4f} П: {t['susp_0_r']:.4f} | Ось1 Л: {t['susp_1_l']:.4f} П: {t['susp_1_r']:.4f}   ")
    print(f" ДВИГАТЕЛЬ:  Обороты (RPM): {t['engine_n']:6.1f} | Работает: {int(t['engine_on'])} | Топливо: {t['engine_tank_content']:5.1f} л   ")
    print(f" ПАНЕЛЬ:     Передача: [{gear}] | Дальний свет: {int(t['lights_fern'])} | Поворотники: {blinkers}                   ")
    print("-" * 80)
