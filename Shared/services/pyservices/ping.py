from services.pyservices.service import Service, OK, FAIL

class Ping(Service):
    def on_execute(self, data: bytearray) -> tuple:
        return (
            "PONG!",
            bytearray(bytes("PONG!", "utf-8")),
            OK
        )