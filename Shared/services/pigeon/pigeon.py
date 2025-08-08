# use pigeon to send stream data to atena
# nest is the tcp socket that's listening
# which is in the C# in AtenaAI

import socket
from loguru import logger

class Pigeon:
    def __init__(self, nest_addr, nest_port) -> None:
        logger.info(nest_addr, nest_port)


PIGEON_INSTANCE = None