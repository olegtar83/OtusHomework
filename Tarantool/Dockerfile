FROM tarantool/tarantool:3.0
RUN apt-get update -y && apt-get upgrade -y && apt-get install -y tarantool-dev libpq-dev git cmake \
    && git clone https://github.com/tarantool/pg.git \
    && cd pg && cmake . -DCMAKE_BUILD_TYPE=RelWithDebugInfo \
    && make && make install && cd ..
ENV TT_WAL_MODE=none