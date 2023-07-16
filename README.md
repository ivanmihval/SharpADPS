# SharpADPS

SharpADPS is a modern sneakernet application. It's free and decentralised, you don't need to have an email or phone number. Using this program you can send messages or any files across the world without the internet. The reason of writing this software was that the last sneakernet software I know was so called "FloppyNet" which means [FidoNet](https://en.wikipedia.org/wiki/FidoNet) for poor people who could not afford a 56k modem in 90s. Today you can afford a 64GB USB-drive for several dollars and send tiny public keys as well as send HD films through this network. It cannot substitute the modern internet but it's good enough for using as an emergency network. The design of this network is so simple that you need just a computer and a removable media, the cost of this equipment is extremely low. The architecture of this network is also easy, so you easily can write your own implementation (for example, see the [Python CLI Implementation Source Code](https://github.com/ivanmihval/PyADPS)).

## Installation

Go to releases, download the archive, extract to a folder and launch the `.exe` file.

## Requirements

`.NET 4.0` (`Windows XP`, `Windows Vista`, `Windows 7`, `Windows 8`, `Windows 10`)

## Usage

See the [Demo section](#demo). If you want to know more you also can read `documentation.txt`.

## Configuration

Usually you don't need configuring this program, but for debug or extra features like "Custom Translation" you need to work with the configuration file. The location of this file depends on OS, you can see the details in the `documentation.txt`.

## Build & Testing

The project is built using `Visual Studio 2010`. Using this IDE you also can run the tests. It's possible that 3rd party libraries links are broken, in this case you should relink them manually, the needed DLLs are in this repository.

## Demo

https://github.com/ivanmihval/SharpADPS/assets/3406377/fc5ab5f3-289c-48d2-8ef5-c37285f83ece

## Author

Ivan Mikhailov (ivanmihval@yandex.ru)

## Website

https://adps-project.org/

## License

MIT
