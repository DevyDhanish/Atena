# don't use this file any more, speech recog is not being done using RealTimeSTTs

import whisper
import tempfile
import scipy.io
from services.common.desktop_audio import DesktopAudio
from collections import deque
from threading import Thread
from loguru import logger

class SpeechToText():
    def __init__(self, language = "en", model_type = "tiny", model_backend = "cuda") -> None:
        self.use_language = language
        self.model = None
        self.all_text = deque()
        # if Atena speech recognition is running slow
        # then pick one of these
        # self.MODEL_TYPE = "tiny"
        # self.MODEL_TYPE = "base"
        # if you have really good gpu
        # use this 
        # self.MODEL_TYEP = "large" 
        self.model_type = model_type

        self.model_thread : Thread

        # if you don't have dedicated gpu with cuda installed
        # then use this line
        # self.MODEL_BACKEND = "cpu"
        self.model_backend = model_backend

        self.desktop_audio = None

        self.should_stop = False

    def add_text(self, text):
        self.all_text.append(text)

    def get_text(self):
        if len(self.all_text) <= 0:
            return None
        
        return self.all_text.popleft()

    def model_init(self):
        self.model = whisper.load_model(name=self.model_type, device=self.model_backend)

    def transcript(self, recording, sample_rate):
        logger.info("Transcribing a audio")
        with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as f:
            scipy.io.wavfile.write(f.name, sample_rate, recording)
            audio_path = f.name

            result = self.model.transcribe(audio_path, language="en")
            txt_str = result["text"]
            logger.info(txt_str)
            self.add_text(txt_str)

    def stop(self):
        self.should_stop = True

    def start_transcribing(self):
        self.model_init()

        # init the model
        # start recording
        # get the recording
        # transcribe
        # store

        self.desktop_audio = DesktopAudio()
        self.desktop_audio.record()

        while not self.should_stop:
            recording = self.desktop_audio.get_audio()

            if recording is not None:
                self.transcript(recording, self.desktop_audio.sample_rate)


    def transcribe(self):
        self.model_thread = Thread(target=self.start_transcribing)   
        self.model_thread.start()