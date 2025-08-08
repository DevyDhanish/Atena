import sounddevice as sd
import numpy as np
import threading
from collections import deque

class DesktopAudio():
    def __init__(self, thresh = 500, dura = 3, sr = 16000, kt = 20) -> None:
        self.threshold = thresh
        self.duration = dura
        self.sample_rate = sr
        self.keep_track = kt
        self.clean_up_count = 0 #remove extra audio
        self.recording_queue = deque()

    def adjust_rec_que(self):
        rec_q_len = len(self.recording_queue)

        if rec_q_len > self.keep_track:
            # only delete what's needed
            overflow = (rec_q_len - self.keep_track)

            # add extra cleanup if specified
            overflow += self.clean_up_count

            for _ in range(overflow):
                if self.recording_queue:
                    self.recording_queue.popleft()

    def add_audio(self, data):
        self.recording_queue.append(data)

    def get_audio(self):
        if len(self.recording_queue) <= 0:
            return None
        
        return self.recording_queue.popleft()
    
    def start_recording(self):
        while True:
            recording = sd.rec(int(self.duration * self.sample_rate), samplerate=self.sample_rate, channels=1, dtype='int16')
            sd.wait()
            self.adjust_rec_que()
            self.add_audio(recording)

    def audio_available(self, data):
        return np.max(np.abs(data)) > self.threshold
    
    def record(self):
        recording_thread = threading.Thread(target=self.start_recording)
        recording_thread.start()
