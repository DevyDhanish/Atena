"""
This file is auto executed my the atena.exe process
execution will start from main(). it handles starting and return the results of various services
"""

from loguru import logger
from concurrent import futures
import grpc
import servicecmd_pb2_grpc as servercmd_grpc
from services import service_handler
from services.config import Config, CONFIG_INSTANCE
from services.pigeon.pigeon import Pigeon, PIGEON_INSTANCE, set_pigeon_instance


class ServiceServer(servercmd_grpc.AtenServicesServicer):
    def ExeService(self, request, context):
        return service_handler.handle_service(request, context)

def main():
    global PIGEON_INSTANCE, CONFIG_INSTANCE

    logger.info("Starting the main service...")  
    
    CONFIG_INSTANCE = Config("config.json")

    nest_addr = CONFIG_INSTANCE.get_nest_addr()
    nest_port = CONFIG_INSTANCE.get_nest_port()
    grpc_server = CONFIG_INSTANCE.get_grpc_server()
    grpc_port = CONFIG_INSTANCE.get_grpc_port()

    set_pigeon_instance(Pigeon(nest_addr, nest_port))

    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    servercmd_grpc.add_AtenServicesServicer_to_server(ServiceServer(), server)

    server_addr = f"{grpc_server}:{grpc_port}"
    logger.info(server_addr)
    try:
        server.add_insecure_port(server_addr)
        server.start()
        logger.info("Grpc Up and Running")
    except RuntimeError:
        logger.error("Another instance of gRPC is already running.")

    server.wait_for_termination()

if __name__ == "__main__":
    main()