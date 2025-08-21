# don't use this file any more, speech recog is not being done using RealTimeSTT



import sounddevice as sd
import numpy as np
import threading
from collections import deque
from loguru import logger

class DesktopAudio():
    def __init__(self, thresh=500, dura=3, sr=16000, kt=20) -> None:
        self.threshold = thresh
        self.duration = dura
        self.sample_rate = sr
        self.keep_track = kt
        self.clean_up_count = 0
        self.recording_queue = deque()
        self.running = True

    def adjust_rec_que(self):
        rec_q_len = len(self.recording_queue)
        if rec_q_len > self.keep_track:
            overflow = (rec_q_len - self.keep_track) + self.clean_up_count
            for _ in range(overflow):
                if self.recording_queue:
                    self.recording_queue.popleft()

    def add_audio(self, data):
        logger.info("Added new audio")
        self.recording_queue.append(data)

    def get_audio(self):
        if len(self.recording_queue) <= 0:
            return None
        return self.recording_queue.popleft()
    
    def start_recording(self):
        # A short duration for "listening" to the mic
        listening_duration = 0.2
        
        while self.running:
            # Continuously record small, non-blocking chunks
            listening_chunk = sd.rec(int(listening_duration * self.sample_rate), 
                                     samplerate=self.sample_rate, channels=1, dtype='int16')
            sd.wait()
            
            # Check if this small chunk has sound
            if self.audio_available(listening_chunk):
                logger.info("Sound detected! Capturing full audio chunk...")
                
                # Now, record a longer chunk to capture the full sound
                full_recording = sd.rec(int(self.duration * self.sample_rate), 
                                        samplerate=self.sample_rate, channels=1, dtype='int16')
                sd.wait()
                
                # Adjust and add the longer recording to the queue
                self.adjust_rec_que()
                self.add_audio(full_recording)
    
    def audio_available(self, data):
        # Check if the max absolute value of the audio data exceeds the threshold
        return np.max(np.abs(data)) > self.threshold
    
    def record(self):
        recording_thread = threading.Thread(target=self.start_recording)
        recording_thread.daemon = True
        recording_thread.start()

    def stop_recording(self):
        self.running = False