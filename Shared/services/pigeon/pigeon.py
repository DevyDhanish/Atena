# use pigeon to send stream data to atena
# nest is the tcp socket that's listening
# which is in the C# in AtenaAI

from loguru import logger
import socket
import base64
import struct
from services.streamdata_pb2 import StreamData

DisplayType_CHAT_UI = 0
DisplayType_MAIN_UI = 1
DisplayType_NEITHER = 3

DataType_AI_GEN_TEXT = 0
DataType_NORMAL_TEXT = 1
DataType_EVENT = 2


class Pigeon:
    _instance = None

    def __new__(cls, *args, **kwargs):
        if cls._instance is not None:
            return cls._instance
        
        cls._instance = super().__new__(cls)
        return cls._instance
        
    def __init__(self, nest_addr=None, nest_port=None) -> None:

        if nest_addr == None and nest_port == None:
            return
        
        logger.info(f"Finding nest at {nest_addr}:{nest_port}")
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.connect((nest_addr, int(nest_port)))
        logger.info(f"Pigeon is in the nest")

    def send_data(self, display_type, data_type, data: bytearray):
        base64_bytes = base64.b64encode(data)

        stream_data = StreamData(
            dataType=data_type,
            displayType=display_type,
            data=base64_bytes
        )

        serialized = stream_data.SerializeToString()

        # Prefix message with 4-byte length (big-endian)
        msg_len = struct.pack(">I", len(serialized))
        self.server_socket.sendall(msg_len + serialized)


# this will be set from main_service.py
#def set_pigeon_instance(instance):
#    global PIGEON_INSTANCE
#    PIGEON_INSTANCE = instance
#
#def get_pigeon_instance() -> Pigeon:
#    if PIGEON_INSTANCE is None:
#        raise RuntimeError("PIGEON_INSTANCE accessed before initialization!")
#    return PIGEON_INSTANCE