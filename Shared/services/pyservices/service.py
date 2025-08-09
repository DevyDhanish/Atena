from abc import abstractmethod, ABC
from services.cmd_packet import ResPacket

# RETURN STATUS
OK = 0
FAIL = 1

# SERVICES
PING = 0
LISTEN_TO_DESKTOP_AUDIO = 1
SEE_SCREEN = 2
DEFAULT_OK_RETURN = 3
DEFAULT_BAD_RETURN = 4
START_SERVICE = 5
STOP_LISTEN_TO_DESKTOP_AUDIO = 6

def get_service_name_by_id(service_id: int) -> str:
    return {
        0: "Ping",
        5: "StartService",
        2: "SeeScreen",
        3: "DefaultOkReturn",
        1: "ListenToDesktopAudio",
        4: "DefaultBadReturn",
        6: "StopListenDesktop",
    }.get(service_id, "Not a service")

class Service(ABC):
    @abstractmethod 
    def on_execute(self, data : bytearray) -> ResPacket:
        pass