import grpc
import servicecmd_pb2 as servicecmd
import servicecmd_pb2_grpc as servercmd_grpc
from loguru import logger
from services.pyservices.service import Service, FAIL, DEFAULT_BAD_RETURN
from services.pyservices.ping import Ping
from services.cmd_packet import CmdPacket, ResPacket

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

    cmdPacket = CmdPacket(request)

    logger.info(f"Got service with id : {cmdPacket.serviceId}")

    match(cmdPacket.serviceId):
        case 0:
            return exec_service(Ping(), cmdPacket.serviceData)
        case _:
            logger.error("No handler for this")
            return create_res(ResPacket(
                DEFAULT_BAD_RETURN,
                "DEFAULT_BAD_RETURN",
                bytearray(bytes("Not a valid service id", "utf-8")),
                FAIL
            ))