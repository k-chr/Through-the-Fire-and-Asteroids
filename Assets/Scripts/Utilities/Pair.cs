using System.Collections.Generic;

public struct Pair<T, U> {
	private T first;
	private U second;

	public Pair(T _first, U _second) {
		this.first = _first;
		this.second = _second;
	}

	public T GetFirst() {
		return this.first;
	}
	
	public U GetSecond() {
		return this.second;
	}
}
