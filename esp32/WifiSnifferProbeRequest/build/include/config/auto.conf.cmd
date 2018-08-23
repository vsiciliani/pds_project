deps_config := \
	/home/SicilianiVi/esp/esp-idf/components/app_trace/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/aws_iot/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/bt/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/driver/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/esp32/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/esp_adc_cal/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/esp_http_client/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/ethernet/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/fatfs/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/freertos/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/heap/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/http_server/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/libsodium/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/log/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/lwip/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/mbedtls/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/mdns/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/openssl/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/pthread/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/spi_flash/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/spiffs/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/tcpip_adapter/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/vfs/Kconfig \
	/home/SicilianiVi/esp/esp-idf/components/wear_levelling/Kconfig \
	/home/SicilianiVi/esp/esp-idf/Kconfig.compiler \
	/home/SicilianiVi/esp/esp-idf/components/bootloader/Kconfig.projbuild \
	/home/SicilianiVi/esp/esp-idf/components/esptool_py/Kconfig.projbuild \
	/home/SicilianiVi/esp/esp-idf/components/partition_table/Kconfig.projbuild \
	/home/SicilianiVi/esp/esp-idf/Kconfig

include/config/auto.conf: \
	$(deps_config)


$(deps_config): ;
