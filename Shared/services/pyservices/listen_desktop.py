from services.cmd_packet import ResPacket
from services.pyservices.service import Service, LISTEN_TO_DESKTOP_AUDIO, get_service_name_by_id, OK
from threading import Thread
from services.pigeon.pigeon import DataType_AI_GEN_TEXT, DataType_NORMAL_TEXT, DisplayType_CHAT_UI, Pigeon
from services.common.speech_to_txt import SpeechToText
from loguru import logger
from services.common.ai_gen import TextGen
from services.config import Config


from RealtimeSTT import AudioToTextRecorder

# def process_text(text):
#     print(text)

# if __name__ == '__main__':
#     print("Wait until it says 'speak now'")
#     recorder = AudioToTextRecorder()

#     while True:
#         recorder.text(process_text)

class ListenDesktop(Service):
    _instance = None

    def __new__(cls, *args, **kwargs):
        if cls._instance is not None:
            logger.info("ListenDesktop instance already exists, using that instead.")
            return cls._instance
        cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self):
        self.recorder = None
        self.text_gen = None
        self.recording_thread = None
        self.stop_flag = False

    def on_recording_start_callback(self):
        logger.info("Recording Started")

    def on_recording_stop_callback(self):
        logger.info("Recording stopped")

    def process_text(self, text):
        # if we are alredy made a text gen model. just reuse that no need to spin up again
        if self.text_gen is None:
            self.text_gen = TextGen()

        # start the text generation thread
        self.text_gen.generate_response(text)

        # while text gen is happening send the transcript back to ui
        pigeon = Pigeon()
        pigeon.send_data(
            DisplayType_CHAT_UI,
            DataType_NORMAL_TEXT,
            bytearray(bytes(text, "utf-8"))
        )

    def stop_recording(self):
        self.stop_flag = True
        if self.recorder is not None and not self.recorder.is_shut_down:
            self.recorder.stop()
            self.recorder.shutdown()
        if self.recording_thread is not None:
            self.recording_thread.join()
            self.recording_thread = None

    def start_transcribing_desktop_audio(self):
        if self.recorder is None:
            config = Config()  # this will return the Config instance

            device_name = config.get_device_name()
            model_name = config.get_model_name()

            self.recorder = AudioToTextRecorder(
                                            device=device_name,
                                            model=model_name,
                                            on_recording_start=self.on_recording_start_callback,
                                            on_recording_stop=self.on_recording_stop_callback)

        self.stop_flag = False

        while not self.stop_flag:
            if self.recorder is not None:
                self.recorder.text(on_transcription_finished=self.process_text)

    def on_execute(self, data: bytearray) -> ResPacket:
        # if we are not already recording
        if self.recording_thread is None or not self.recording_thread.is_alive(): 
            self.recording_thread = Thread(target=self.start_transcribing_desktop_audio)
            self.recording_thread.start()

        return ResPacket(
            LISTEN_TO_DESKTOP_AUDIO,
            get_service_name_by_id(LISTEN_TO_DESKTOP_AUDIO),
            bytearray(bytes("listening", "utf-8")),
            OK
        )