from loguru import logger

class CmdPacket:
    def __init__(self, packet) -> None:
        self.serviceId = packet.serviceId
        self.serviceName = packet.serviceName
        self.serviceData = bytearray(packet.data)


class ResPacket:
    def __init__(self, id : int, name : str, data : bytearray, status : int):
        self.serviceId = id
        self.serviceName = name
        self.serviceData = data
        self.serviceStatus = status