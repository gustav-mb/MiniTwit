import { useDocumentTitle } from "../utilities/Utilities";

function NotFound() {
    useDocumentTitle("404")

    return (
        <>
            <center>
                <h2><strong>404</strong></h2>
                <h3>The requested page does not exist!</h3>
            </center>
        </>
    );
}

export default NotFound