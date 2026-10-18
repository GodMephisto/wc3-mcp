---
type: vendor-doc
source_repo: "https://github.com/gavrilovicm/JASS"
source_path: "matrix.cpp"
ref: HEAD
---

#include"matrix.hpp"

Matrix::Matrix(std::vector<std::vector<double>> m){
	this->m_matrix= m;
}

std::vector<std::vector<double>> Matrix::getMatrix(){
	return this->m_matrix;
}

void Matrix::add_to_matrix(std::vector<double> m){
	this->m_matrix.push_back(m);
}

std::ostream& operator<<(std::ostream& out, Matrix m){
	out << "Matrix:" << std::endl;
	std::vector<std::vector<double>> p = m.getMatrix();
	unsigned n, k, i, j;

	n = p.size();
	k = p[0].size();


	out << '[';
	for (i = 0; i<n-1; ++i){
		// if is for aligning every column
		if (i==0){
			out << '[';
		}else{
			out << " [";
		}
		for (j = 0; j<k-1; ++j){
			out << p[i][j] << ", ";
		}
		out << p[i][j] << ']';
		out << std::endl;
		if (i == n-2) out << ' ';
	}
	out << '[';
	for (j = 0; j<k-1; ++j){
		out << p[i][j] << ", ";
	}
	out << p[i][j] << ']';
	out << ']';

	return out;
}
