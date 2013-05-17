#include <iostream>

int main() {
	int i = 0;
	
	if (i == 0) {
		std::cout << "test";
	}
	
	switch(i) {
		case 0:
		std::cout << "test";
	}
	
	while (i != 0) {
		std::cout << "test";
	}
	
	do {
		std::cout << "test";
	} while (i != 0);
	
	for (i = 0; i < 0; i++) {
		std::cout << "test";
	}
	return 0;
}
