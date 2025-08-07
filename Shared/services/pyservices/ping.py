from services.pyservices.service import Service, OK, PING
from services.cmd_packet import ResPacket

class Ping(Service):
    def on_execute(self, data: bytearray) -> ResPacket:
        return ResPacket(
            PING,
            "PONG!",
            bytearray(bytes("PONG!!", "utf-8")),
            OK
        )