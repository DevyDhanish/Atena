import sounddevice as sd
import numpy as np
import whisper
import tempfile
import scipy.io.wavfile
import threading
from collections import deque

THRESHOLD = 500
DURATION = 2
SAMPLE_RATE = 16000

print(sd.query_devices())  # lists all audio devices

print("Default input device:", sd.default.device[0])


def is_loud(data):
    return np.max(np.abs(data)) > THRESHOLD

model = whisper.load_model("small", device="cuda")  # use "base" if you wanna flex GPU

print("Listening for noise...")

recordings = deque()

TEXT = ""

def transcript(recording):
    global TEXT
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as f:
        scipy.io.wavfile.write(f.name, SAMPLE_RATE, recording)
        audio_path = f.name

        result = model.transcribe(audio_path, language="en")
        txt_str = result["text"]

        print(txt_str)

def start():
    if len(recordings) == 0: return

    latest_recording = recordings.pop()

    transcript(latest_recording)

def startAILop():
    while True:
        start()

t = threading.Thread(target=startAILop)

has_started = False

while True:
    audio = sd.rec(int(0.5 * SAMPLE_RATE), samplerate=SAMPLE_RATE, channels=1, dtype='int16')
    sd.wait()
    if is_loud(audio): 
        recording = sd.rec(int(DURATION * SAMPLE_RATE), samplerate=SAMPLE_RATE, channels=1, dtype='int16')
        sd.wait()

        recordings.append(recording)

        if not has_started:
            t.start()
            has_started = True