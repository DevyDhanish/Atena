from services.cmd_packet import ResPacket
from services.pyservices.service import Service, LISTEN_TO_DESKTOP_AUDIO, get_service_name_by_id, OK
from threading import Thread
from services.common.speech_to_txt import SpeechToText
from loguru import logger

class ListenDesktop(Service):
    def start_transcribing_desktop_audio(self):
        stt = SpeechToText()
        stt.transcribe()

        while True:
            text = stt.get_text()

            if text is not None:
                logger.info(text)

    def on_execute(self, data: bytearray) -> ResPacket:
        Thread(target=self.start_transcribing_desktop_audio).start()

        return ResPacket(
            LISTEN_TO_DESKTOP_AUDIO,
            get_service_name_by_id(LISTEN_TO_DESKTOP_AUDIO),
            bytearray(bytes("listening", "utf-8")),
            OK
        )
    

ld = ListenDesktop().on_execute(bytearray(bytes("he", "utf-8")))