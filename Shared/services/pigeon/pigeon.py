# use pigeon to send stream data to atena
# nest is the tcp socket that's listening
# which is in the C# in AtenaAI

import socket
from loguru import logger
from services import streamdata_pb2

DisplayType_CHAT_UI = 0
DisplayType_MAIN_UI = 1

DataType_AI_GEN_TEXT = 0
DataType_NORMAL_TEXT = 1


class Pigeon:
    def __init__(self, nest_addr, nest_port) -> None:
        logger.info(f"Finding nest at {nest_addr}:{nest_port}")
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        port = int(nest_port)
        self.server_socket.connect((nest_addr, port))

        logger.info(f"Pigeon is in the nest")

    # def send_data(self, display_type, data_type, data : bytearray):
    #     stream_data = streamdata_pb2.StreamData(
    #         dataType = data_type
    #         displayType = display_type
    #         data = bytes(data)
    #     )

# this will be set from main_service.py
PIGEON_INSTANCE = None