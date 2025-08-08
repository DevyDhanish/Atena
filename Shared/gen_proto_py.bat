@echo off

python -m grpc_tools.protoc -Iproto\ --python_out=. --grpc_python_out=. proto\servicecmd.proto

python -m grpc_tools.protoc -Iproto\ --python_out=services\ proto\streamdata.proto