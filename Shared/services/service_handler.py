import grpc
import servicecmd_pb2 as servicecmd
import servicecmd_pb2_grpc as servercmd_grpc
from loguru import logger
from services.pyservices.service import Service
from services.pyservices.ping import Ping


def create_res(name : str, data : bytearray, status: int):
    return servicecmd.CmdResponse(
        serviceName=name,
        data=bytes(data),
        status=status
    )

def exec_service(service : Service, data : bytearray):
    ret = service.on_execute(data)

    return create_res(ret[0], ret[1], ret[2])

def handle_service(request, context):
    logger.info(f"Got service with id : {request.serviceId}")

    match(request.serviceId):
        case 0:
            return exec_service(Ping(), bytearray(request.data))

        case _:
            logger.error("No handler for this")