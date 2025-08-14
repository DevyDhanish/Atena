import grpc
import servicecmd_pb2 as servicecmd
import servicecmd_pb2_grpc as servercmd_grpc
from loguru import logger
from services.pyservices.service import Service, get_service_name_by_id, FAIL, OK, DEFAULT_BAD_RETURN, PING, LISTEN_TO_DESKTOP_AUDIO, STOP_LISTEN_TO_DESKTOP_AUDIO
from services.pyservices.ping import Ping
from services.pyservices.listen_desktop import ListenDesktop
from services.cmd_packet import CmdPacket, ResPacket
from services.common.ai_gen import TextGen

def create_res(serviceRes: ResPacket):
    return servicecmd.CmdResponse(
        serviceId=serviceRes.serviceId if serviceRes.serviceId is not None else DEFAULT_BAD_RETURN,
        serviceName=serviceRes.serviceName or "ERROR",
        data=bytes(serviceRes.serviceData) if serviceRes.serviceData else b"ERROR",
        status=serviceRes.serviceStatus if serviceRes.serviceStatus is not None else FAIL
    )

def exec_service(service : Service, data : bytearray):
    ret : ResPacket = service.on_execute(data)
    ret = create_res(ret)

    return ret

# return packet
def handle_service(request, context):
    cmd_packet = CmdPacket(request)
    logger.info(f"Got service with ID: {cmd_packet.serviceId}")

    global LISTEN_DESKTOP_INSTANCE

    match cmd_packet.serviceId:
        case 0:
            return exec_service(Ping(), cmd_packet.serviceData)

        case 1:
            # Create or reuse the ListenDesktop singleton instance
            return exec_service(ListenDesktop(), cmd_packet.serviceData)

        case 6:
            listen_desk_inst = ListenDesktop()
            if listen_desk_inst is not None:
                listen_desk_inst.stop_recording()
                logger.info("Stopped desktop listening service")
            else:
                logger.warning("Stop requested, but ListenDesktop instance does not exist")
                return create_res(ResPacket(
                    STOP_LISTEN_TO_DESKTOP_AUDIO,
                    get_service_name_by_id(STOP_LISTEN_TO_DESKTOP_AUDIO),
                    bytearray("Not Recording, no need to stop", "utf-8"),
                    OK
                ))
            
            return create_res(ResPacket(
                STOP_LISTEN_TO_DESKTOP_AUDIO,
                get_service_name_by_id(STOP_LISTEN_TO_DESKTOP_AUDIO),
                bytearray("Stopped listening to desktop audio", "utf-8"),
                OK
            ))

        case _:
            logger.error("No handler for this service ID")
            return create_res(ResPacket(
                DEFAULT_BAD_RETURN,
                "DEFAULT_BAD_RETURN",
                bytearray("Not a valid service ID", "utf-8"),
                FAIL
            ))
