from services.cmd_packet import ResPacket
from services.pyservices.service import Service, LISTEN_TO_DESKTOP_AUDIO, get_service_name_by_id, OK
from threading import Thread
from services.pigeon.pigeon import PIGEON_INSTANCE, get_pigeon_instance, DataType_AI_GEN_TEXT, DataType_NORMAL_TEXT, DisplayType_CHAT_UI
from services.common.speech_to_txt import SpeechToText
from loguru import logger
from services.common.ai_gen import TextGen


from RealtimeSTT import AudioToTextRecorder

# def process_text(text):
#     print(text)

# if __name__ == '__main__':
#     print("Wait until it says 'speak now'")
#     recorder = AudioToTextRecorder()

#     while True:
#         recorder.text(process_text)

LISTEN_DESKTOP_INSTANCE = None

class ListenDesktop(Service):
    _instance = None

    def __new__(cls, *args, **kwargs):
        if cls._instance is not None:
            logger.error("ListenDesktop instance already exists, use that instead.")
            return cls._instance
        cls._instance = super().__new__(cls)
        return cls._instance

    def __init__(self):
        if hasattr(self, "_initialized") and self._initialized:
            return  # Prevent re-running init
        self.recorder = None
        self.should_stop = False
        self._initialized = True
        self.text_gen = None

    def on_recording_start_callback(self):
        logger.info("Recording Started")

    def on_recording_stop_callback(self):
        logger.info("Recording stopped")

    def process_text(self, text):

        self.text_gen = TextGen()
        self.text_gen.generate_response(text)

        get_pigeon_instance().send_data(
            DisplayType_CHAT_UI,
            DataType_NORMAL_TEXT,
            bytearray(bytes(text, "utf-8"))
        )

    def stop_recording(self):
        if not self.recorder.is_shut_down:
            self.recorder.shutdown()
            self.should_stop = True
            return
        
        logger.info("Recorder is not running")

    def start_transcribing_desktop_audio(self):
        self.should_stop = False

        if self.recorder is not None : return

        self.recorder = AudioToTextRecorder(device="cuda",
                                             model="tiny",
                                             on_recording_start=self.on_recording_start_callback,
                                             on_recording_stop=self.on_recording_stop_callback)
        
        while not self.should_stop:
            self.recorder.text(on_transcription_finished=self.process_text)

    def on_execute(self, data: bytearray) -> ResPacket:
        Thread(target=self.start_transcribing_desktop_audio).start()

        return ResPacket(
            LISTEN_TO_DESKTOP_AUDIO,
            get_service_name_by_id(LISTEN_TO_DESKTOP_AUDIO),
            bytearray(bytes("listening", "utf-8")),
            OK
        )