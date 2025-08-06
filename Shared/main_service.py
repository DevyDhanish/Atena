"""
This file is auto executed my the atena.exe process
execution will start from main(). it handles starting and return the results of various services
"""

from loguru import logger
from concurrent import futures
import grpc
import servicecmd_pb2 as servicecmd
import servicecmd_pb2_grpc as servercmd_grpc
from services import service_handler


class ServiceServer(servercmd_grpc.AtenServicesServicer):
    def ExeService(self, request, context):

        return service_handler.handle_service(request, context)

def main():
    logger.info("Starting the main service...")  
    
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    servercmd_grpc.add_AtenServicesServicer_to_server(ServiceServer(), server)

    try:
        server.add_insecure_port('localhost:50051')
        server.start()
    except RuntimeError:
        logger.error("Another instance of gRPC is already running.")

    logger.success("gRPC Server running on port 50051")
    server.wait_for_termination()

if __name__ == "__main__":
    main()